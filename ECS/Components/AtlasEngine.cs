using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Messages;
using Atlas.Core.Objects;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Messages;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas.ECS.Components
{
	public sealed class AtlasEngine : AtlasComponent<IEngine>, IEngine
	{
		#region Static Singleton

		private static AtlasEngine instance;

		/// <summary>
		/// Creates a singleton instance of the Engine.
		/// Only one Engine should exist at a time.
		/// </summary>
		public static AtlasEngine Instance
		{
			get { return instance = instance ?? new AtlasEngine(); }
		}

		#endregion

		private readonly Group<IEntity> entities = new Group<IEntity>();
		private readonly Group<IFamily> families = new Group<IFamily>();
		private readonly Group<ISystem> systems = new Group<ISystem>();

		private readonly Dictionary<string, IEntity> entitiesGlobalName = new Dictionary<string, IEntity>();
		private readonly Dictionary<Type, IFamily> familiesType = new Dictionary<Type, IFamily>();
		private readonly Dictionary<Type, ISystem> systemsType = new Dictionary<Type, ISystem>();

		private readonly Dictionary<Type, Type> systemsInstance = new Dictionary<Type, Type>();

		private readonly Dictionary<Type, int> familiesReference = new Dictionary<Type, int>();
		private readonly Dictionary<Type, int> systemsReference = new Dictionary<Type, int>();

		private readonly Stack<IReadOnlyFamily> familiesRemoved = new Stack<IReadOnlyFamily>();
		private readonly Stack<IReadOnlySystem> systemsRemoved = new Stack<IReadOnlySystem>();

		private readonly Stopwatch timer = new Stopwatch();
		private IReadOnlySystem currentSystem;

		private bool isRunning = false;
		private TimeStep updateState = TimeStep.None;

		private double maxVariableTime = 0.25;
		private double deltaVariableTime = 0;
		private double totalVariableTime = 0;

		private double deltaFixedTime = 1d / 60d;
		private double totalFixedTime = 0;

		private AtlasEngine() { }

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			entity.AddListener<IChildAddMessage>(EntityChildAdded, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IChildRemoveMessage>(EntityChildRemoved, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IGlobalNameMessage>(EntityGlobalNameChanged, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IComponentAddMessage>(EntityComponentAdded, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IComponentRemoveMessage>(EntityComponentRemoved, int.MinValue, MessageHierarchy.All);
			entity.AddListener<ISystemTypeAddMessage>(EntitySystemAdded, int.MinValue, MessageHierarchy.All);
			entity.AddListener<ISystemTypeRemoveMessage>(EntitySystemRemoved, int.MinValue, MessageHierarchy.All);
			AddEntity(entity);
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			RemoveEntity(entity);
			entity.RemoveListener<IChildAddMessage>(EntityChildAdded);
			entity.RemoveListener<IChildRemoveMessage>(EntityChildRemoved);
			entity.RemoveListener<IGlobalNameMessage>(EntityGlobalNameChanged);
			entity.RemoveListener<IComponentAddMessage>(EntityComponentAdded);
			entity.RemoveListener<IComponentRemoveMessage>(EntityComponentRemoved);
			entity.RemoveListener<ISystemTypeAddMessage>(EntitySystemAdded);
			entity.RemoveListener<ISystemTypeRemoveMessage>(EntitySystemRemoved);
			base.RemovingManager(entity, index);
		}

		public IReadOnlyGroup<IEntity> Entities { get { return entities; } }
		public IReadOnlyGroup<IReadOnlyFamily> Families { get { return families; } }
		public IReadOnlyGroup<IReadOnlySystem> Systems { get { return systems; } }

		#region Entities

		public bool HasEntity(string globalName)
		{
			return !string.IsNullOrWhiteSpace(globalName) && entitiesGlobalName.ContainsKey(globalName);
		}

		public bool HasEntity(IEntity entity)
		{
			return entity != null && entitiesGlobalName.ContainsKey(entity.GlobalName) && entitiesGlobalName[entity.GlobalName] == entity;
		}

		public IEntity GetEntity(string globalName)
		{
			return entitiesGlobalName.ContainsKey(globalName) ? entitiesGlobalName[globalName] : null;
		}

		private void AddEntity(IEntity entity)
		{
			//Change the global name of an incoming Entity if it already exists.
			if(entitiesGlobalName.ContainsKey(entity.GlobalName))
				entity.GlobalName = AtlasEntity.UniqueName;

			entitiesGlobalName.Add(entity.GlobalName, entity);
			entities.Add(entity);
			entity.Engine = this;

			//TO-DO
			//entity.Systems isn't an EngineList, so it might screw up.
			foreach(var type in entity.Systems)
				AddSystem(type);

			foreach(var family in families)
				family.AddEntity(entity);

			Dispatch<IEntityAddMessage>(new EntityAddMessage(this, entity));

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

			Dispatch<IEntityRemoveMessage>(new EntityRemoveMessage(this, entity));

			//TO-DO
			//entity.Systems isn't an EngineList, so it might screw up.
			foreach(var type in entity.Systems)
				RemoveSystem(type);

			foreach(var family in families)
				family.RemoveEntity(entity);

			entitiesGlobalName.Remove(entity.GlobalName);
			entities.Remove(entity);
			entity.Engine = null;
		}

		private void EntityChildAdded(IChildAddMessage message)
		{
			AddEntity(message.Value);
		}

		private void EntityChildRemoved(IChildRemoveMessage message)
		{
			if(message.Value.Parent == null)
				RemoveEntity(message.Value);
		}

		private void EntityGlobalNameChanged(IGlobalNameMessage message)
		{
			entitiesGlobalName.Remove(message.PreviousValue);
			entitiesGlobalName.Add(message.CurrentValue, message.Messenger);
		}

		#endregion

		#region Systems

		public bool AddSystemType<TISystem, TSystem>()
			where TISystem : ISystem
			where TSystem : TISystem, new()
		{
			return AddSystemType(typeof(TISystem), typeof(TSystem));
		}

		public bool AddSystemType(Type type, Type instance)
		{
			if(type == null)
				return false;
			if(instance == null)
				return false;
			if(!type.IsInterface)
				return false;
			if(!instance.IsClass)
				return false;
			if(type == typeof(ISystem))
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(!type.IsAssignableFrom(instance))
				return false;
			if(instance.GetConstructor(Type.EmptyTypes) == null)
				return false;
			if(systemsInstance.ContainsKey(type) && systemsInstance[type] == instance)
				return false;
			RemoveSystemType(type);
			systemsInstance.Add(type, instance);
			AddSystem(type);
			return true;
		}

		public bool RemoveSystemType<TISystem>()
			where TISystem : IReadOnlySystem
		{
			return RemoveSystemType(typeof(TISystem));
		}

		public bool RemoveSystemType(Type type)
		{
			if(type == null)
				return false;
			if(!type.IsInterface)
				return false;
			if(type == typeof(IReadOnlySystem))
				return false;
			if(!typeof(IReadOnlySystem).IsAssignableFrom(type))
				return false;
			if(!systemsInstance.ContainsKey(type))
				return false;
			systemsInstance.Remove(type);
			RemoveSystem(type);
			return true;
		}

		private void AddSystem(Type type)
		{
			if(!systemsReference.ContainsKey(type))
				return;
			//There's no system instance class assigned.
			if(!systemsInstance.ContainsKey(type))
				return;
			ISystem system;
			try
			{
				system = Activator.CreateInstance(systemsInstance[type]) as ISystem;
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
				return;
			}

			systemsType.Add(type, system);

			system.AddListener<IPriorityMessage>(SystemPriorityChanged);
			SystemPriorityChanged(system);

			system.Engine = this;
			Dispatch<ISystemAddMessage>(new SystemAddMessage(this, type, system));
		}

		private void RemoveSystem(Type type)
		{
			if(!systemsType.ContainsKey(type))
				return;
			var system = systemsType[type];

			system.RemoveListener<IPriorityMessage>(SystemPriorityChanged);
			systems.Remove(system);

			systemsType.Remove(type);
			Dispatch<ISystemRemoveMessage>(new SystemRemoveMessage(this, type, system));
			if(updateState != TimeStep.None)
			{
				systemsRemoved.Push(system);
			}
			else
			{
				system.Dispose();
			}
		}

		private void EntitySystemAdded(ISystemTypeAddMessage message)
		{
			if(!systemsReference.ContainsKey(message.Value))
			{
				systemsReference.Add(message.Value, 1);
				AddSystem(message.Value);
			}
			else
				++systemsReference[message.Value];
		}

		private void EntitySystemRemoved(ISystemTypeRemoveMessage message)
		{
			if(--systemsReference[message.Value] > 0)
				return;
			systemsReference.Remove(message.Value);
			RemoveSystem(message.Value);
		}

		private void SystemPriorityChanged(IPriorityMessage message)
		{
			SystemPriorityChanged(message.Messenger as ISystem);
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

		public bool HasSystem(IReadOnlySystem system)
		{
			return systems.Contains(system as ISystem);
		}

		public bool HasSystem<TISystem>() where TISystem : IReadOnlySystem
		{
			return HasSystem(typeof(TISystem));
		}

		public bool HasSystem(Type type)
		{
			return systemsType.ContainsKey(type);
		}

		public TISystem GetSystem<TISystem>() where TISystem : IReadOnlySystem
		{
			return (TISystem)GetSystem(typeof(TISystem));
		}

		public IReadOnlySystem GetSystem(Type type)
		{
			return systemsType.ContainsKey(type) ? systemsType[type] : null;
		}

		public IReadOnlySystem GetSystem(int index)
		{
			return systems[index];
		}

		#endregion

		#region Updates

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

		public TimeStep UpdateState
		{
			get { return updateState; }
			private set
			{
				if(updateState == value)
					return;
				var previous = updateState;
				updateState = value;
				Dispatch<IUpdateStateMessage<IEngine>>(new UpdateStateMessage<IEngine>(this, value, previous));
			}
		}

		public IReadOnlySystem CurrentSystem
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

		public bool IsRunning
		{
			get { return isRunning; }
			set
			{
				if(isRunning == value)
					return;
				isRunning = value;
				//Only run again when the last Update()/timer is done.
				//If the Engine is turned off and on during an Update()
				//loop, while(isRunning) will catch it.
				if(value && !timer.IsRunning)
				{
					timer.Restart();
					var previousTime = 0d;
					while(isRunning)
					{
						var currentTime = timer.Elapsed.TotalSeconds;

						var deltaVariableTime = currentTime - previousTime;
						previousTime = currentTime;

						//Cap delta time to avoid the "spiral of death".
						if(deltaVariableTime > maxVariableTime)
							deltaVariableTime = maxVariableTime;

						var deltaFixedTime = DeltaFixedTime;
						var totalFixedTime = TotalFixedTime;

						//Calculate the number of fixed updates.
						var fixedUpdates = 0;
						while(totalFixedTime + deltaFixedTime < totalVariableTime)
						{
							totalFixedTime += deltaFixedTime;
							++fixedUpdates;
						}

						//Update all delta and total times.
						DeltaVariableTime = deltaVariableTime;
						TotalVariableTime = totalVariableTime + deltaVariableTime;
						TotalFixedTime = totalFixedTime;

						//Run fixed-time and variable-time updates.
						while(fixedUpdates-- > 0)
							UpdateSystems(TimeStep.Fixed, deltaFixedTime);
						UpdateSystems(TimeStep.Variable, deltaVariableTime);

						DestroySystems();
						DestroyFamilies();
					}
					DeltaVariableTime = 0;
					timer.Stop();
				}
			}
		}

		private void UpdateSystems(TimeStep timeStep, double deltaTime)
		{
			UpdateState = timeStep;
			foreach(var system in systems)
			{
				if(system.TimeStep != timeStep)
					continue;
				CurrentSystem = system;
				system.Update(deltaTime);
				CurrentSystem = null;
			}
			UpdateState = TimeStep.None;
		}

		private void DestroySystems()
		{
			while(systemsRemoved.Count > 0)
				systemsRemoved.Pop().Dispose();
		}

		#endregion

		#region Families

		public bool HasFamily(IReadOnlyFamily family)
		{
			return familiesType.ContainsValue(family as IFamily);
		}

		public bool HasFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			return HasFamily(typeof(TFamilyMember));
		}

		public bool HasFamily(Type type)
		{
			return familiesType.ContainsKey(type);
		}

		public IReadOnlyFamily<TFamilyMember> AddFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesType.ContainsKey(type))
			{
				var family = new AtlasFamily<TFamilyMember>();

				families.Add(family);
				familiesType.Add(type, family);
				familiesReference.Add(type, 1);
				family.Engine = this;

				foreach(var entity in entities)
					family.AddEntity(entity);
				Dispatch<IFamilyAddMessage>(new FamilyAddMessage(this, type, family));
				return family;
			}
			else
			{
				++familiesReference[type];
				return familiesType[type] as IFamily<TFamilyMember>;
			}
		}

		public IReadOnlyFamily<TFamilyMember> RemoveFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesType.ContainsKey(type))
				return null;
			var family = familiesType[type];
			if(--familiesReference[type] > 0)
				return family as IReadOnlyFamily<TFamilyMember>;
			families.Remove(family);
			familiesType.Remove(type);
			familiesReference.Remove(type);
			Dispatch<IFamilyRemoveMessage>(new FamilyRemoveMessage(this, type, family));
			if(updateState != TimeStep.None)
			{
				familiesRemoved.Push(family);
			}
			else
			{
				family.Dispose();
			}
			return family as IReadOnlyFamily<TFamilyMember>;
		}

		private void DestroyFamilies()
		{
			while(familiesRemoved.Count > 0)
				familiesRemoved.Pop().Dispose();
		}

		public IReadOnlyFamily<TFamilyMember> GetFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			return GetFamily(typeof(TFamilyMember)) as IFamily<TFamilyMember>;
		}

		public IReadOnlyFamily GetFamily(Type type)
		{
			return familiesType.ContainsKey(type) ? familiesType[type] : null;
		}

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
	}
}