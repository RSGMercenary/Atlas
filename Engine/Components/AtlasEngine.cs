using Atlas.Engine.Collections.EngineList;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Messages;
using Atlas.Engine.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas.Engine.Components
{
	sealed public class AtlasEngine : AtlasComponent, IEngine
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

		private EngineList<IEntity> entities = new EngineList<IEntity>();
		private EngineList<IFamily> families = new EngineList<IFamily>();
		private EngineList<ISystem> systems = new EngineList<ISystem>();

		private Dictionary<string, IEntity> entitiesGlobalName = new Dictionary<string, IEntity>();
		private Dictionary<Type, IFamily> familiesType = new Dictionary<Type, IFamily>();
		private Dictionary<Type, ISystem> systemsType = new Dictionary<Type, ISystem>();

		private Dictionary<Type, Type> systemsInstance = new Dictionary<Type, Type>();

		private Dictionary<Type, int> familiesCount = new Dictionary<Type, int>();
		private Dictionary<Type, int> systemsCount = new Dictionary<Type, int>();

		private Stack<IFamily> familiesRemoved = new Stack<IFamily>();
		private Stack<ISystem> systemsRemoved = new Stack<ISystem>();

		private bool isRunning = false;
		private Stopwatch timer = new Stopwatch();
		private UpdatePhase updateState = UpdatePhase.None;
		private bool updateLock = true;
		private ISystem currentSystem;

		private double deltaUpdateTime = 0;
		private double totalUpdateTime = 0;

		private double deltaFixedUpdateTime = 1d / 60d;
		private double totalFixedUpdateTime = 0;

		private AtlasEngine()
		{

		}

		override protected void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			entity.AddListener<IChildAddMessage>(EntityChildAdded, int.MinValue);
			entity.AddListener<IChildRemoveMessage>(EntityChildRemoved, int.MinValue);
			entity.AddListener<IGlobalNameMessage>(EntityGlobalNameChanged, int.MinValue);
			entity.AddListener<IComponentAddMessage>(EntityComponentAdded, int.MinValue);
			entity.AddListener<IComponentRemoveMessage>(EntityComponentRemoved, int.MinValue);
			entity.AddListener<ISystemTypeAddMessage>(EntitySystemAdded, int.MinValue);
			entity.AddListener<ISystemTypeRemoveMessage>(EntitySystemRemoved, int.MinValue);
			AddEntity(entity);
		}

		override protected void RemovingManager(IEntity entity, int index)
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

		protected override void Destroying()
		{
			//Null out Engine singleton to allow
			//another to be instantiated.
			if(instance == this)
				instance = null;
			base.Destroying();
		}

		public IReadOnlyEngineList<IEntity> Entities { get { return entities; } }
		public IReadOnlyEngineList<IFamily> Families { get { return families as IReadOnlyEngineList<IFamily>; } }
		public IReadOnlyEngineList<ISystemBase> Systems { get { return systems as IReadOnlyEngineList<ISystemBase>; } }

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

			foreach(IEntity child in entity.Children.Backward())
				RemoveEntity(child);

			Message<IEntityRemoveMessage>(new EntityRemoveMessage(entity));

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

		public double DeltaUpdateTime
		{
			get { return deltaUpdateTime; }
			private set
			{
				if(deltaUpdateTime == value)
					return;
				deltaUpdateTime = value;
			}
		}

		public double TotalUpdateTime
		{
			get { return totalUpdateTime; }
			private set
			{
				if(totalUpdateTime == value)
					return;
				totalUpdateTime = value;
			}
		}

		public double DeltaFixedUpdateTime
		{
			get { return deltaFixedUpdateTime; }
			set
			{
				if(deltaFixedUpdateTime == value)
					return;
				deltaFixedUpdateTime = value;
			}
		}

		public double TotalFixedUpdateTime
		{
			get { return totalFixedUpdateTime; }
			private set
			{
				if(totalFixedUpdateTime == value)
					return;
				totalFixedUpdateTime = value;
			}
		}

		public UpdatePhase UpdateState
		{
			get { return updateState; }
			private set
			{
				if(updateState == value)
					return;
				var previous = updateState;
				updateState = value;
				Message<IUpdatePhaseMessage>(new UpdatePhaseMessage(value, previous));
			}
		}

		public ISystem CurrentSystem
		{
			get { return currentSystem; }
			private set
			{
				if(currentSystem == value)
					return;
				//If a Signal/Message were toever be put here, do it before the set.
				//Prevents System.Update() or System.FixedUpdate() from being mis-called.
				currentSystem = value;
			}
		}

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
			where TISystem : ISystem
		{
			return RemoveSystemType(typeof(TISystem));
		}

		public bool RemoveSystemType(Type type)
		{
			if(type == null)
				return false;
			if(!type.IsInterface)
				return false;
			if(type == typeof(ISystem))
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(!systemsInstance.ContainsKey(type))
				return false;
			systemsInstance.Remove(type);
			RemoveSystem(type);
			return true;
		}

		private void AddSystem(Type type)
		{
			if(!systemsCount.ContainsKey(type))
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
			SystemPriorityChanged(system, system.Priority, 0);
			system.Engine = this;
			Message<ISystemAddMessage>(new SystemAddMessage(type, system));
		}

		private void RemoveSystem(Type type)
		{
			if(!systemsType.ContainsKey(type))
				return;
			var system = systemsType[type];
			system.RemoveListener<IPriorityMessage>(SystemPriorityChanged);
			systems.Remove(system);
			systemsType.Remove(type);
			Message<ISystemRemoveMessage>(new SystemRemoveMessage(type, system));
			if(UpdateState == UpdatePhase.None)
			{
				system.Destroy();
			}
			else
			{
				systemsRemoved.Push(system);
			}
		}

		private void EntitySystemAdded(ISystemTypeAddMessage message)
		{
			if(!systemsCount.ContainsKey(message.Value))
			{
				systemsCount.Add(message.Value, 1);
				AddSystem(message.Value);
			}
			else
				++systemsCount[message.Value];
		}

		private void EntitySystemRemoved(ISystemTypeRemoveMessage message)
		{
			if(--systemsCount[message.Value] > 0)
				return;
			systemsCount.Remove(message.Value);
			RemoveSystem(message.Value);
		}

		private void SystemPriorityChanged(IPriorityMessage message)
		{
			SystemPriorityChanged(message.Messenger, message.Messenger.Priority, -1);
		}

		private void SystemPriorityChanged(ISystem system, int next, int previous)
		{
			systems.Remove(system);

			for(var index = systems.Count; index > 0; --index)
			{
				if(systems[index - 1].Priority <= next)
				{
					systems.Insert(index, system);
					return;
				}
			}

			systems.Insert(0, system);
		}

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

						//This is mainly for debugging.
						//Stopwatch is accurate, but doesn't stop for breakpoints.
						if(currentTime - previousTime > 1)
						{
							previousTime = currentTime;
							continue;
						}

						DeltaUpdateTime = currentTime - previousTime;

						updateLock = false;
						FixedUpdate(DeltaUpdateTime);

						updateLock = false;
						Update(DeltaUpdateTime);

						DestroySystems();
						DestroyFamilies();

						previousTime = currentTime;
					}
					DeltaUpdateTime = 0;
					TotalUpdateTime = 0;
					TotalFixedUpdateTime = 0;
					timer.Stop();
				}
			}
		}

		public void FixedUpdate(double deltaTime)
		{
			if(updateLock)
				return;
			updateLock = true;

			var deltaFixedUpdateTime = DeltaFixedUpdateTime;
			var totalFixedUpdateTime = TotalFixedUpdateTime;
			var totalUpdateTime = TotalUpdateTime + deltaTime;

			var fixedUpdates = 0;
			while(totalFixedUpdateTime + deltaFixedUpdateTime < totalUpdateTime)
			{
				totalFixedUpdateTime += deltaFixedUpdateTime;
				++fixedUpdates;
			}

			UpdateState = UpdatePhase.FixedUpdate;

			while(fixedUpdates-- > 0)
			{
				foreach(var system in systems)
				{
					CurrentSystem = system;
					system.FixedUpdate(deltaFixedUpdateTime);
					CurrentSystem = null;
				}
			}

			UpdateState = UpdatePhase.None;

			TotalFixedUpdateTime = totalFixedUpdateTime;
		}

		public void Update(double deltaTime)
		{
			if(updateLock)
				return;
			updateLock = true;

			UpdateState = UpdatePhase.Update;

			foreach(var system in systems)
			{
				CurrentSystem = system;
				system.Update(deltaTime);
				CurrentSystem = null;
			}

			UpdateState = UpdatePhase.None;

			TotalUpdateTime += deltaTime;
		}

		private void DestroySystems()
		{
			while(systemsRemoved.Count > 0)
				systemsRemoved.Pop().Destroy();
		}

		#endregion

		#region Families

		public bool HasFamily(IFamily family)
		{
			return familiesType.ContainsValue(family);
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

		public IFamily<TFamilyMember> AddFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesType.ContainsKey(type))
			{
				var family = new AtlasFamily<TFamilyMember>();

				families.Add(family);
				familiesType.Add(type, family);
				familiesCount.Add(type, 1);
				family.Engine = this;

				foreach(var entity in entities)
					family.AddEntity(entity);
				Message<IFamilyAddMessage>(new FamilyAddMessage(type, family));
				return family;
			}
			else
			{
				++familiesCount[type];
				return familiesType[type] as IFamily<TFamilyMember>;
			}
		}

		public IFamily<TFamilyMember> RemoveFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesType.ContainsKey(type))
				return null;
			var family = familiesType[type];
			if(--familiesCount[type] > 0)
				return family as IFamily<TFamilyMember>;
			families.Remove(family);
			familiesType.Remove(type);
			familiesCount.Remove(type);
			Message<IFamilyRemoveMessage>(new FamilyRemoveMessage(type, family));
			if(UpdateState == UpdatePhase.None)
			{
				family.Destroy();
			}
			else
			{
				familiesRemoved.Push(family);
			}
			return family as IFamily<TFamilyMember>;
		}

		private void DestroyFamilies()
		{
			while(familiesRemoved.Count > 0)
				familiesRemoved.Pop().Destroy();
		}

		public IFamily<TFamilyMember> GetFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			return GetFamily(typeof(TFamilyMember)) as IFamily<TFamilyMember>;
		}

		public IFamily GetFamily(Type type)
		{
			return familiesType.ContainsKey(type) ? familiesType[type] : null;
		}

		private void EntityComponentAdded(IComponentAddMessage message)
		{
			foreach(IFamily family in families)
				family.AddEntity(message.Messenger, message.Key);
		}

		private void EntityComponentRemoved(IComponentRemoveMessage message)
		{
			foreach(IFamily family in families)
				family.RemoveEntity(message.Messenger, message.Key);
		}

		#endregion
	}
}