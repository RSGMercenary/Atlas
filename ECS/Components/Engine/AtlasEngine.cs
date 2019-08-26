using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Messages;
using Atlas.Core.Objects;
using Atlas.Core.Utilites;
using Atlas.ECS.Components.Messages;
using Atlas.ECS.Entities;
using Atlas.ECS.Entities.Messages;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Atlas.ECS.Systems.Messages;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components
{
	public class AtlasEngine : AtlasComponent<IEngine>, IEngine
	{
		#region Fields

		//ECS Groups
		private readonly Group<IEntity> entities = new Group<IEntity>();
		private readonly Group<IFamily> families = new Group<IFamily>();
		private readonly Group<ISystem> systems = new Group<ISystem>();

		//ECS Dictionaries
		private readonly Dictionary<string, IEntity> entitiesGlobalName = new Dictionary<string, IEntity>();
		private readonly Dictionary<Type, IFamily> familiesType = new Dictionary<Type, IFamily>();
		private readonly Dictionary<Type, ISystem> systemsType = new Dictionary<Type, ISystem>();

		//Reference Counting
		private readonly Dictionary<Type, int> familiesReference = new Dictionary<Type, int>();
		private readonly Dictionary<Type, int> systemsReference = new Dictionary<Type, int>();

		//Update State
		private bool updateLock = false;
		private ISystem currentSystem;
		private TimeStep updateState = TimeStep.None;

		//Variable Time
		private double maxVariableTime = 0.25f;
		private double deltaVariableTime = 0;
		private double totalVariableTime = 0;

		//Fixed Time
		private int lagFixedTime = 0;
		private double deltaFixedTime = 1d / 60d;
		private double totalFixedTime = 0;

		#endregion

		#region Compose/Dispose

		protected override void AddingManager(IEntity entity, int index)
		{
			if(entity.Root != entity)
				throw new InvalidOperationException($"The {GetType().Name} must be added to the root {nameof(IEntity)}.");
			base.AddingManager(entity, index);
			entity.AddListener<IChildAddMessage>(EntityChildAdded, int.MinValue, Tree.All);
			entity.AddListener<IRootMessage>(EntityRootChanged, int.MinValue, Tree.All);
			entity.AddListener<IGlobalNameMessage>(EntityGlobalNameChanged, int.MinValue, Tree.All);
			entity.AddListener<IComponentAddMessage>(EntityComponentAdded, int.MinValue, Tree.All);
			entity.AddListener<IComponentRemoveMessage>(EntityComponentRemoved, int.MinValue, Tree.All);
			AddEntity(entity);
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			RemoveEntity(entity);
			entity.RemoveListener<IChildAddMessage>(EntityChildAdded);
			entity.RemoveListener<IRootMessage>(EntityRootChanged);
			entity.RemoveListener<IGlobalNameMessage>(EntityGlobalNameChanged);
			entity.RemoveListener<IComponentAddMessage>(EntityComponentAdded);
			entity.RemoveListener<IComponentRemoveMessage>(EntityComponentRemoved);
			base.RemovingManager(entity, index);
		}

		#endregion

		#region Entities

		#region Add/Remove

		private void AddEntity(IEntity entity)
		{
			//Change the Entity's global name if it already exists.
			if(entitiesGlobalName.ContainsKey(entity.GlobalName))
				entity.GlobalName = AtlasEntity.UniqueName;

			entitiesGlobalName.Add(entity.GlobalName, entity);
			entities.Add(entity);
			entity.Engine = this;

			foreach(var family in families)
				family.AddEntity(entity);

			Message<IEntityAddMessage>(new EntityAddMessage(entity));

			foreach(var child in entity.Children.Forward())
				AddEntity(child);
		}

		private void RemoveEntity(IEntity entity)
		{
			//Protect against parents signaling a child being removed which never got to be added.
			if(!entitiesGlobalName.ContainsKey(entity.GlobalName) ||
				entitiesGlobalName[entity.GlobalName] != entity)
				return;

			foreach(var child in entity.Children.Backward())
				RemoveEntity(child);

			Message<IEntityRemoveMessage>(new EntityRemoveMessage(entity));

			foreach(var family in families)
				family.RemoveEntity(entity);

			entitiesGlobalName.Remove(entity.GlobalName);
			entities.Remove(entity);
			entity.Engine = null;
		}

		#endregion

		#region Get

		public IReadOnlyGroup<IEntity> Entities { get { return entities; } }

		public IEntity GetEntity(string globalName)
		{
			return entitiesGlobalName.ContainsKey(globalName) ? entitiesGlobalName[globalName] : null;
		}

		#endregion

		#region Has

		public bool HasEntity(string globalName)
		{
			return !string.IsNullOrWhiteSpace(globalName) && entitiesGlobalName.ContainsKey(globalName);
		}

		public bool HasEntity(IEntity entity)
		{
			return entity != null && entitiesGlobalName.ContainsKey(entity.GlobalName) && entitiesGlobalName[entity.GlobalName] == entity;
		}

		#endregion

		#region Events

		private void EntityChildAdded(IChildAddMessage message)
		{
			if(!entitiesGlobalName.ContainsKey(message.Value.GlobalName) ||
				entitiesGlobalName[message.Value.GlobalName] != message.Value)
				AddEntity(message.Value);
		}

		private void EntityRootChanged(IRootMessage message)
		{
			if(message.CurrentValue == null)
				RemoveEntity(message.Messenger);
		}

		private void EntityGlobalNameChanged(IGlobalNameMessage message)
		{
			entitiesGlobalName.Remove(message.PreviousValue);
			entitiesGlobalName.Add(message.CurrentValue, message.Messenger);
		}

		#endregion

		#endregion

		#region Systems

		#region Create

		protected virtual ISystem CreateSystem(Type type)
		{
			return (ISystem)Activator.CreateInstance(type);
		}

		#endregion

		#region Add/Remove

		public TSystem AddSystem<TSystem>()
			where TSystem : class, ISystem, new()
		{
			return AddSystem(typeof(TSystem)) as TSystem;
		}

		public ISystem AddSystem(Type type)
		{
			if(!systemsReference.ContainsKey(type))
			{
				var system = CreateSystem(type);
				system.AddListener<IPriorityMessage<ISystem>>(SystemPriorityChanged);
				SystemPriorityChanged(system);
				systemsType.Add(type, system);
				systemsReference.Add(type, 1);
				system.Engine = this;
				Message<ISystemAddMessage>(new SystemAddMessage(type, system));
			}
			else
			{
				++systemsReference[type];
			}
			return systemsType[type];
		}

		public void RemoveSystem<TSystem>()
			where TSystem : class, ISystem, new()
		{
			RemoveSystem(typeof(TSystem));
		}

		public void RemoveSystem(Type type)
		{
			if(!systemsReference.ContainsKey(type))
				return;
			if(--systemsReference[type] > 0)
				return;
			var system = systemsType[type];
			system.RemoveListener<IPriorityMessage<ISystem>>(SystemPriorityChanged);
			systems.Remove(system);
			systemsType.Remove(type);
			systemsReference.Remove(type);
			system.Engine = null;
			Message<ISystemRemoveMessage>(new SystemRemoveMessage(type, system));
		}

		#endregion

		#region Get

		public IReadOnlyGroup<ISystem> Systems { get { return systems; } }

		public TISystem GetSystem<TISystem>() where TISystem : ISystem
		{
			return (TISystem)GetSystem(typeof(TISystem));
		}

		public ISystem GetSystem(Type type)
		{
			return systemsType.ContainsKey(type) ? systemsType[type] : null;
		}

		public ISystem GetSystem(int index)
		{
			return systems[index];
		}

		#endregion

		#region Has

		public bool HasSystem(ISystem system)
		{
			return systems.Contains(system);
		}

		public bool HasSystem<TISystem>() where TISystem : ISystem
		{
			return HasSystem(typeof(TISystem));
		}

		public bool HasSystem(Type type)
		{
			return systemsType.ContainsKey(type);
		}

		#endregion

		#region Events

		private void SystemPriorityChanged(IPriorityMessage<ISystem> message)
		{
			SystemPriorityChanged(message.Messenger);
		}

		private void SystemPriorityChanged(ISystem system)
		{
			Priority.Prioritize(systems, system);
		}

		#endregion

		#endregion

		#region Families

		#region Create

		protected virtual IFamily<TFamilyMember> CreateFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new()
		{
			return new AtlasFamily<TFamilyMember>();
		}

		#endregion

		#region Add/Remove

		public IFamily<TFamilyMember> AddFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesType.ContainsKey(type))
			{
				var family = CreateFamily<TFamilyMember>();
				families.Add(family);
				familiesType.Add(type, family);
				familiesReference.Add(type, 1);
				family.Engine = this;
				foreach(var entity in entities)
					family.AddEntity(entity);
				Message<IFamilyAddMessage>(new FamilyAddMessage(type, family));
			}
			else
			{
				++familiesReference[type];
			}
			return (IFamily<TFamilyMember>)familiesType[type];
		}

		public void RemoveFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesReference.ContainsKey(type))
				return;
			if(--familiesReference[type] > 0)
				return;
			var family = familiesType[type];
			families.Remove(family);
			familiesType.Remove(type);
			familiesReference.Remove(type);
			family.Engine = null;
			Message<IFamilyRemoveMessage>(new FamilyRemoveMessage(type, family));
		}

		#endregion

		#region Get

		public IReadOnlyGroup<IFamily> Families { get { return families; } }

		public IFamily<TFamilyMember> GetFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new()
		{
			return GetFamily(typeof(TFamilyMember)) as IFamily<TFamilyMember>;
		}

		public IFamily GetFamily(Type type)
		{
			return familiesType.ContainsKey(type) ? familiesType[type] : null;
		}

		#endregion

		#region Has

		public bool HasFamily(IFamily family)
		{
			return familiesType.ContainsValue(family);
		}

		public bool HasFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new()
		{
			return HasFamily(typeof(TFamilyMember));
		}

		public bool HasFamily(Type type)
		{
			return familiesType.ContainsKey(type);
		}

		#endregion

		#region Events

		private void EntityComponentAdded(IComponentAddMessage message)
		{
			foreach(var family in families)
				family.AddEntity(message.Messenger, message.Key);
		}

		private void EntityComponentRemoved(IComponentRemoveMessage message)
		{
			foreach(var family in families)
				family.RemoveEntity(message.Messenger, message.Key);
		}

		#endregion

		#endregion

		#region Updates

		#region Delta/total Times

		public double MaxVariableTime
		{
			get { return maxVariableTime; }
			set
			{
				if(maxVariableTime == value)
					return;
				maxVariableTime = value;
			}
		}

		public double DeltaVariableTime
		{
			get { return deltaVariableTime; }
			private set
			{
				if(deltaVariableTime == value)
					return;
				deltaVariableTime = value;
			}
		}

		public double TotalVariableTime
		{
			get { return totalVariableTime; }
			private set
			{
				if(totalVariableTime == value)
					return;
				totalVariableTime = value;
			}
		}

		public double DeltaFixedTime
		{
			get { return deltaFixedTime; }
			set
			{
				if(deltaFixedTime == value)
					return;
				deltaFixedTime = value;
			}
		}

		public double TotalFixedTime
		{
			get { return totalFixedTime; }
			private set
			{
				if(totalFixedTime == value)
					return;
				totalFixedTime = value;
			}
		}

		#endregion

		#region State

		public TimeStep UpdateState
		{
			get { return updateState; }
			private set
			{
				if(updateState == value)
					return;
				var previous = updateState;
				updateState = value;
				Message<IUpdateStateMessage<IEngine>>(new UpdateStateMessage<IEngine>(value, previous));
			}
		}

		public ISystem CurrentSystem
		{
			get { return currentSystem; }
			private set
			{
				if(currentSystem == value)
					return;
				//If a Signal/Message were to ever be put here, do it before the set.
				//Prevents System.Update() from being mis-called.
				currentSystem = value;
			}
		}

		#endregion

		#region Fixed/Variable Update Loop

		public void Update(double deltaTime)
		{
			if(updateLock)
				return;
			updateLock = true;

			var deltaVariableTime = deltaTime;

			//Cap delta time to avoid the "spiral of death".
			if(deltaVariableTime > maxVariableTime)
				deltaVariableTime = maxVariableTime;

			var deltaFixedTime = DeltaFixedTime;
			var totalFixedTime = TotalFixedTime;

			//Calculate the number of fixed updates.
			var fixedUpdates = 0;
			while(totalFixedTime + deltaFixedTime <= totalVariableTime)
			{
				totalFixedTime += deltaFixedTime;
				++fixedUpdates;
			}

			//Calculate when fixed-time and variable-time updates weren't 1:1.
			//Let the Systems decide how to use the value.
			lagFixedTime += Math.Max(0, fixedUpdates - 1);
			if(fixedUpdates == 1 && lagFixedTime > 0)
				--lagFixedTime;

			//Update all delta and total times.
			DeltaVariableTime = deltaVariableTime;
			TotalVariableTime = totalVariableTime + deltaVariableTime;
			TotalFixedTime = totalFixedTime;

			//Run fixed-time and variable-time updates.
			while(fixedUpdates-- > 0)
				UpdateSystems(TimeStep.Fixed, deltaFixedTime);
			UpdateSystems(TimeStep.Variable, deltaVariableTime);

			updateLock = false;
		}

		private void UpdateSystems(TimeStep timeStep, double deltaTime)
		{
			UpdateState = timeStep;
			foreach(var system in systems)
			{
				if(system.TimeStep != timeStep)
					continue;
				CurrentSystem = system;
				system.Update((float)deltaTime);
				CurrentSystem = null;
			}
			UpdateState = TimeStep.None;
		}

		#endregion

		#endregion
	}
}