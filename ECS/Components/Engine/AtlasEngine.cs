using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Loggers;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine
{
	public class AtlasEngine : AtlasComponent<IEngine>, IEngine, IUpdate<double>
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
		private UpdateLock UpdateLock { get; } = new UpdateLock();
		private TimeStep updateState = TimeStep.None;
		private ISystem updateSystem;

		//Variable Time
		private double deltaVariableTime = 0;
		private double totalVariableTime = 0;
		private double maxVariableTime = 0.25d;
		private double variableInterpolation = 0;

		//Fixed Time
		private double deltaFixedTime = 1d / 60d;
		private double totalFixedTime = 0;
		private int fixedUpdates = 0;
		private int fixedLag = 0;
		#endregion

		#region Compose/Dispose
		protected override void AddingManager(IEntity entity, int index)
		{
			if(!entity.IsRoot)
				throw new InvalidOperationException($"An {nameof(IEngine)} can't be added to a non-root {nameof(IEntity)}.");

			base.AddingManager(entity, index);
			entity.AddListener<IChildAddMessage<IEntity>>(EntityChildAdded, int.MinValue, Relation.All);
			entity.AddListener<IRootMessage<IEntity>>(EntityRootChanged, int.MinValue, Relation.All);
			entity.AddListener<IGlobalNameMessage>(EntityGlobalNameChanged, int.MinValue, Relation.All);
			entity.AddListener<IComponentAddMessage>(EntityComponentAdded, int.MinValue, Relation.All);
			entity.AddListener<IComponentRemoveMessage>(EntityComponentRemoved, int.MinValue, Relation.All);
			AddEntity(entity);
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			RemoveEntity(entity);
			entity.RemoveListener<IChildAddMessage<IEntity>>(EntityChildAdded);
			entity.RemoveListener<IRootMessage<IEntity>>(EntityRootChanged);
			entity.RemoveListener<IGlobalNameMessage>(EntityGlobalNameChanged);
			entity.RemoveListener<IComponentAddMessage>(EntityComponentAdded);
			entity.RemoveListener<IComponentRemoveMessage>(EntityComponentRemoved);
			base.RemovingManager(entity, index);
		}
		#endregion

		public bool HasObject(IEngineObject instance)
		{
			if(instance is IEntity entity)
				return HasEntity(entity);
			if(instance is ISystem system)
				return HasSystem(system);
			if(instance is IFamily family)
				return HasFamily(family);
			return false;
		}

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
		public IReadOnlyGroup<IEntity> Entities => entities;

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

		#region Messages
		private void EntityChildAdded(IChildAddMessage<IEntity> message)
		{
			if(!entitiesGlobalName.ContainsKey(message.Value.GlobalName) ||
				entitiesGlobalName[message.Value.GlobalName] != message.Value)
				AddEntity(message.Value);
		}

		private void EntityRootChanged(IRootMessage<IEntity> message)
		{
			if(message.Messenger == Manager)
				RemoveManager(message.Messenger);
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
		protected virtual ISystem CreateSystem(Type type) => (ISystem)Activator.CreateInstance(type);
		#endregion

		#region Add/Remove
		public TSystem AddSystem<TSystem>()
			where TSystem : class, ISystem, new() => AddSystem(typeof(TSystem)) as TSystem;

		public ISystem AddSystem(Type type)
		{
			if(!systemsReference.ContainsKey(type))
			{
				var system = CreateSystem(type);
				system.AddListener<IPriorityMessage>(SystemPriorityChanged);
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

		public void RemoveSystem<TSystem>() where TSystem : class, ISystem, new() => RemoveSystem(typeof(TSystem));

		public void RemoveSystem(Type type)
		{
			if(!systemsReference.ContainsKey(type))
				return;
			if(--systemsReference[type] > 0)
				return;
			var system = systemsType[type];
			system.RemoveListener<IPriorityMessage>(SystemPriorityChanged);
			systems.Remove(system);
			systemsType.Remove(type);
			systemsReference.Remove(type);
			system.Engine = null;
			Message<ISystemRemoveMessage>(new SystemRemoveMessage(type, system));
		}
		#endregion

		#region Get
		public IReadOnlyGroup<ISystem> Systems => systems;

		public TISystem GetSystem<TISystem>() where TISystem : ISystem => (TISystem)GetSystem(typeof(TISystem));

		public ISystem GetSystem(Type type) => systemsType.ContainsKey(type) ? systemsType[type] : null;

		public ISystem GetSystem(int index) => systems[index];
		#endregion

		#region Has
		public bool HasSystem(ISystem system) => systems.Contains(system);

		public bool HasSystem<TISystem>() where TISystem : ISystem => HasSystem(typeof(TISystem));

		public bool HasSystem(Type type) => systemsType.ContainsKey(type);
		#endregion

		#region Messages
		private void SystemPriorityChanged(IPriorityMessage message)
		{
			SystemPriorityChanged(message.Messenger);
		}

		private void SystemPriorityChanged(ISystem system)
		{
			systems.Remove(system);
			for(var index = systems.Count; index > 0; --index)
			{
				if(systems[index - 1].Priority <= system.Priority)
				{
					systems.Insert(index, system);
					return;
				}
			}
			systems.Insert(0, system);
		}
		#endregion
		#endregion

		#region Families
		#region Create
		protected virtual IFamily<TFamilyMember> CreateFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new() => new AtlasFamily<TFamilyMember>();
		#endregion

		#region Add/Remove
		public IReadOnlyFamily<TFamilyMember> AddFamily<TFamilyMember>()
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
			return (IReadOnlyFamily<TFamilyMember>)familiesType[type];
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
		public IReadOnlyGroup<IReadOnlyFamily> Families => families;

		public IReadOnlyFamily<TFamilyMember> GetFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new()
		{
			return GetFamily(typeof(TFamilyMember)) as IReadOnlyFamily<TFamilyMember>;
		}

		public IReadOnlyFamily GetFamily(Type type) => familiesType.ContainsKey(type) ? familiesType[type] : null;
		#endregion

		#region Has
		public bool HasFamily(IReadOnlyFamily family) => familiesType.ContainsValue(family as IFamily);

		public bool HasFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new()
		{
			return HasFamily(typeof(TFamilyMember));
		}

		public bool HasFamily(Type type) => familiesType.ContainsKey(type);
		#endregion

		#region Messages
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
		#region Delta/Total Times
		public double MaxVariableTime
		{
			get => maxVariableTime;
			set
			{
				if(maxVariableTime == value)
					return;
				maxVariableTime = value;
			}
		}

		public double DeltaVariableTime
		{
			get => deltaVariableTime;
			private set
			{
				//Cap delta time to avoid the "spiral of death".
				value = Math.Min(value, maxVariableTime);
				if(deltaVariableTime == value)
					return;
				deltaVariableTime = value;
			}
		}

		public double TotalVariableTime
		{
			get => totalVariableTime;
			private set
			{
				if(totalVariableTime == value)
					return;
				totalVariableTime = value;
			}
		}

		public double DeltaFixedTime
		{
			get => deltaFixedTime;
			set
			{
				if(deltaFixedTime == value)
					return;
				deltaFixedTime = value;
			}
		}

		public double TotalFixedTime
		{
			get => totalFixedTime;
			private set
			{
				if(totalFixedTime == value)
					return;
				totalFixedTime = value;
			}
		}
		#endregion

		#region State
		public int FixedLag
		{
			get => fixedLag;
			private set => fixedLag = value;
		}

		public int FixedUpdates
		{
			get => fixedUpdates;
			private set => fixedUpdates = value;
		}

		public double VariableInterpolation
		{
			get => variableInterpolation;
			private set => variableInterpolation = value;
		}

		public TimeStep UpdateState
		{
			get => updateState;
			private set
			{
				if(updateState == value)
					return;
				var previous = updateState;
				updateState = value;
				Message<IUpdateStateMessage<IEngine>>(new UpdateStateMessage<IEngine>(value, previous));
			}
		}

		public ISystem UpdateSystem
		{
			get => updateSystem;
			private set
			{
				if(updateSystem == value)
					return;
				//If a Signal/Message were to ever be put here, do it before the set.
				//Prevents System.Update() from being mis-called.
				updateSystem = value;
			}
		}
		#endregion

		#region Fixed/Variable Update Loop
		public void Update(double deltaTime)
		{
			try
			{
				UpdateLock.Lock();

				//Variable-time cap and set.
				DeltaVariableTime = deltaTime;
				//Fixed-time cache to avoid modification during update.
				var deltaFixedTime = DeltaFixedTime;

				//Fixed-time updates
				CalculateFixedUpdates(deltaFixedTime);
				CalculateFixedLag();
				while(FixedUpdates > 0)
				{
					TotalFixedTime += deltaFixedTime;
					UpdateSystems(TimeStep.Fixed, deltaFixedTime);
					FixedUpdates--;
				}

				//Variable-time updates
				TotalVariableTime += deltaVariableTime;
				CalculateVariableInterpolation(deltaFixedTime);
				UpdateSystems(TimeStep.Variable, deltaVariableTime);

				UpdateLock.Unlock();
			}
			catch(Exception e)
			{
				Log.Exception(e);
				throw;
			}
		}

		private void CalculateFixedUpdates(double deltaFixedTime)
		{
			var fixedUpdates = 0;
			var totalFixedTime = TotalFixedTime;
			while(totalFixedTime + deltaFixedTime <= totalVariableTime + deltaVariableTime)
			{
				totalFixedTime += deltaFixedTime;
				++fixedUpdates;
			}
			FixedUpdates = fixedUpdates;
		}

		private void CalculateFixedLag()
		{
			//Calculate when fixed-time and variable-time updates weren't 1:1.
			var fixedLag = FixedLag + Math.Max(0, fixedUpdates - 1);
			if(fixedUpdates == 1 && fixedLag > 0)
				--fixedLag;
			FixedLag = fixedLag;
		}

		private void CalculateVariableInterpolation(double deltaFixedTime)
		{
			VariableInterpolation = (TotalVariableTime - TotalFixedTime) / deltaFixedTime;
		}

		private void UpdateSystems(TimeStep timeStep, double deltaTime)
		{
			UpdateState = timeStep;
			foreach(var system in systems)
			{
				if(system.UpdateStep != timeStep)
					continue;
				UpdateSystem = system;
				system.Update((float)deltaTime);
				UpdateSystem = null;
			}
			UpdateState = TimeStep.None;
		}
		#endregion
		#endregion
	}
}