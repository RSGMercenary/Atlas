using Atlas.Engine.Collections.Fixed;
using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Messages;
using Atlas.Engine.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas.Engine.Components
{
	sealed class AtlasEngine : AtlasComponent, IEngine
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

		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private LinkList<IFamily> families = new LinkList<IFamily>();
		private LinkList<ISystem> systems = new LinkList<ISystem>();

		private Dictionary<string, IEntity> entitiesGlobalName = new Dictionary<string, IEntity>();
		private Dictionary<Type, IFamily> familiesType = new Dictionary<Type, IFamily>();
		private Dictionary<Type, ISystem> systemsType = new Dictionary<Type, ISystem>();

		private FixedStack<IEntity> entityPool = new FixedStack<IEntity>();
		private FixedStack<IFamily> familyPool = new FixedStack<IFamily>();
		private Dictionary<Type, Type> systemsInstance = new Dictionary<Type, Type>();

		private Dictionary<Type, int> familiesCount = new Dictionary<Type, int>();
		private Dictionary<Type, int> systemsCount = new Dictionary<Type, int>();

		private List<IFamily> familiesRemoved = new List<IFamily>();
		private List<ISystem> systemsRemoved = new List<ISystem>();

		private bool isRunning = false;
		private Stopwatch timer = new Stopwatch();
		private UpdatePhase updatePhase = UpdatePhase.None;
		private bool updateLock = true;
		private ISystem currentSystem;

		private double deltaUpdateTime = 0;
		private double totalUpdateTime = 0;

		private double deltaFixedUpdateTime = (double)1 / 60;
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
			//Not sure about this one...
			//Null out Engine singleton to allow
			//another to be instantiated.
			if(instance == this)
				instance = null;
			base.Destroying();
		}

		protected override void Resetting()
		{
			base.Resetting();
		}

		public FixedStack<IEntity> EntityPool { get { return entityPool; } }
		public FixedStack<IFamily> FamilyPool { get { return familyPool; } }

		public IReadOnlyLinkList<IEntity> Entities { get { return entities; } }
		public IReadOnlyLinkList<IFamily> Families { get { return families; } }
		public IReadOnlyLinkList<ISystem> Systems { get { return systems; } }

		#region Entities

		public IEntity GetEntity(bool managed = true, string globalName = "", string localName = "")
		{
			IEntity entity;
			if(entityPool.Count > 0)
			{
				entity = entityPool.Pop();
			}
			else
			{
				entity = new AtlasEntity();
			}
			if(managed)
			{
				entity.AddListener<IEngineStateMessage>(EntityStateChanged, int.MinValue);
			}
			entity.GlobalName = globalName;
			entity.LocalName = localName;
			return entity;
		}

		private void EntityStateChanged(IEngineStateMessage message)
		{
			if(!message.AtTarget)
				return;
			IEntity entity = message.Target as IEntity;
			if(entity.State != EngineObjectState.Destroyed)
				return;
			entity.RemoveListener<IEngineStateMessage>(EntityStateChanged);
			entityPool.Push(entity);
		}

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
			if(entitiesGlobalName.ContainsKey(entity.GlobalName))
			{
				if(entitiesGlobalName[entity.GlobalName] == entity)
					//A child added at the end of Entity.Children and signaled
					//in mid-iteration of AddEntity() is already handled.
					return;
				entity.GlobalName = AtlasEntity.UniqueName;
			}

			entitiesGlobalName.Add(entity.GlobalName, entity);
			entities.Add(entity);
			entity.Engine = this;

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
			foreach(IEntity child in entity.Children.Backward())
				RemoveEntity(child);

			Message<IEntityRemoveMessage>(new EntityRemoveMessage(entity));

			foreach(var type in entity.Systems)
				RemoveSystem(type);

			foreach(var family in families)
				family.RemoveEntity(entity);

			//Double check that Entity wasn't removed during message/event.
			//Don't want to remove a different Entity that now has the same name.
			//...But what if it gets added back with the same name? Hmmm...
			if(entitiesGlobalName.ContainsKey(entity.GlobalName))
				if(entitiesGlobalName[entity.GlobalName] != entity)
					return;

			entitiesGlobalName.Remove(entity.GlobalName);
			Debug.WriteLine("Engine removing " + entity.GlobalName);
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
			if(!message.AtTarget)
				return;
			entitiesGlobalName.Remove(message.PreviousValue);
			entitiesGlobalName.Add(message.CurrentValue, message.Target);
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

		public UpdatePhase UpdatePhase
		{
			get { return updatePhase; }
			private set
			{
				if(updatePhase == value)
					return;
				var previous = updatePhase;
				updatePhase = value;
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
			where TSystem : TISystem
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
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(!type.IsAssignableFrom(instance))
				return false;
			if(systemsInstance.ContainsKey(type) && systemsInstance[type] == instance)
				return false;
			int count = systemsCount[type];
			RemoveSystem(type, count);
			systemsInstance.Add(type, instance);
			//We only add a system instance if there is a count of Entities requesting it.
			if(systemsCount.ContainsKey(type))
				AddSystem(type, count);
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
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(!systemsInstance.ContainsKey(type))
				return false;
			systemsInstance.Remove(type);
			if(systemsCount.ContainsKey(type))
				RemoveSystem(type);
			return true;
		}

		private void AddSystem(Type type, int count = 1)
		{
			if(!systemsCount.ContainsKey(type))
			{
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
				systemsCount.Add(type, count);
				systemsType.Add(type, system);
				system.AddListener<IPriorityMessage>(SystemPriorityChanged);
				SystemPriorityChanged(system, system.Priority, 0);
				system.Engine = this;
				Message<ISystemAddMessage>(new SystemAddMessage(type, system));
			}
			else
			{
				systemsCount[type] += count;
			}
		}

		private void RemoveSystem(Type type, int count = 1)
		{
			systemsCount[type] -= count;
			if(systemsCount[type] > 0)
				return;
			var system = systemsType[type];
			system.RemoveListener<IPriorityMessage>(SystemPriorityChanged);
			systems.Remove(system);
			systemsType.Remove(type);
			systemsCount.Remove(type);
			Message<ISystemRemoveMessage>(new SystemRemoveMessage(type, system));
			if(UpdatePhase != UpdatePhase.None)
			{
				systemsRemoved.Add(system);
			}
			else
			{
				DestroySystem(system);
			}
		}

		private void EntitySystemAdded(ISystemTypeAddMessage message)
		{
			if(!message.AtTarget)
				return;
			AddSystem(message.Value);
		}

		private void EntitySystemRemoved(ISystemTypeRemoveMessage message)
		{
			if(!message.AtTarget)
				return;
			RemoveSystem(message.Value);
		}

		private void SystemPriorityChanged(IPriorityMessage message)
		{
			SystemPriorityChanged(message.Target, message.Target.Priority, -1);
		}

		private void SystemPriorityChanged(ISystem system, int next, int previous)
		{
			systems.Remove(system);

			int index = systems.Count;
			for(var current = systems.Last; current != null; current = current.Previous)
			{
				if(current.Value.Priority <= next)
				{
					systems.Add(system, index);
					return;
				}
				--index;
			}

			systems.Add(system, 0);
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

			UpdatePhase = UpdatePhase.FixedUpdate;

			while(fixedUpdates-- > 0)
			{
				foreach(var system in systems)
				{
					CurrentSystem = system;
					system.FixedUpdate(deltaFixedUpdateTime);
					CurrentSystem = null;
				}
			}

			UpdatePhase = UpdatePhase.None;

			TotalFixedUpdateTime = totalFixedUpdateTime;
		}

		public void Update(double deltaTime)
		{
			if(updateLock)
				return;
			updateLock = true;

			UpdatePhase = UpdatePhase.Update;

			foreach(var system in systems)
			{
				CurrentSystem = system;
				system.Update(deltaTime);
				CurrentSystem = null;
			}

			UpdatePhase = UpdatePhase.None;

			TotalUpdateTime += deltaTime;
		}

		private void DestroySystems()
		{
			while(systemsRemoved.Count > 0)
			{
				var system = systemsRemoved[systemsRemoved.Count - 1];
				systemsRemoved.RemoveAt(systemsRemoved.Count - 1);
				DestroySystem(system);
			}
		}

		private void DestroySystem(ISystem system)
		{
			system.Destroy();
		}

		#endregion

		#region Families

		public bool HasFamily(IFamily family)
		{
			if(family == null)
				return false;
			Type type = family.FamilyType;
			return familiesType.ContainsKey(type) && familiesType[type] == family;
		}

		public bool HasFamily<TFamilyType>()
		{
			return HasFamily(typeof(TFamilyType));
		}

		public bool HasFamily(Type type)
		{
			return familiesType.ContainsKey(type);
		}

		public IFamily AddFamily<TFamilyType>()
		{
			return AddFamily(typeof(TFamilyType));
		}

		public IFamily AddFamily(Type type)
		{
			if(!familiesType.ContainsKey(type))
			{
				IFamily family;
				if(familyPool.Count > 0)
				{
					family = familyPool.Pop();
				}
				else
				{
					family = new AtlasFamily();
				}

				families.Add(family);
				familiesType.Add(type, family);
				familiesCount.Add(type, 1);
				family.FamilyType = type;
				family.Engine = this;

				foreach(var entity in entities)
					family.AddEntity(entity);
				Message<IFamilyAddMessage>(new FamilyAddMessage(type, family));
				return family;
			}
			else
			{
				++familiesCount[type];
				return familiesType[type];
			}
		}

		public IFamily RemoveFamily<TFamilyType>()
		{
			return RemoveFamily(typeof(TFamilyType));
		}

		public IFamily RemoveFamily(Type type)
		{
			if(!familiesType.ContainsKey(type))
				return null;
			IFamily family = familiesType[type];
			if(--familiesCount[type] > 0)
				return family;
			families.Remove(family);
			familiesType.Remove(type);
			familiesCount.Remove(type);
			Message<IFamilyRemoveMessage>(new FamilyRemoveMessage(type, family));
			if(UpdatePhase != UpdatePhase.None)
			{
				familiesRemoved.Add(family);
			}
			else
			{
				DestroyFamily(family);
			}
			return family;
		}

		private void DestroyFamilies()
		{
			while(familiesRemoved.Count > 0)
			{
				var family = familiesRemoved[familiesRemoved.Count - 1];
				familiesRemoved.RemoveAt(familiesRemoved.Count - 1);
				DestroyFamily(family);
			}
		}

		private void DestroyFamily(IFamily family)
		{
			family.Destroy();
			familyPool.Push(family);
		}

		public IFamily GetFamily<TFamilyType>()
		{
			return GetFamily(typeof(TFamilyType));
		}

		public IFamily GetFamily(Type type)
		{
			return familiesType.ContainsKey(type) ? familiesType[type] : null;
		}

		private void EntityComponentAdded(IComponentAddMessage message)
		{
			if(!message.AtTarget)
				return;
			foreach(IFamily family in families)
				family.AddEntity(message.Target, message.Key);
		}

		private void EntityComponentRemoved(IComponentRemoveMessage message)
		{
			if(!message.AtTarget)
				return;
			foreach(IFamily family in families)
				family.RemoveEntity(message.Target, message.Key);
		}

		#endregion
	}
}