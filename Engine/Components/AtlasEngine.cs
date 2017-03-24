using Atlas.Engine.Collections.Fixed;
using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Signals;
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
		/// Creates a singleton instance of the Engine. Only one
		/// engine should exist at a time.
		/// </summary>
		public static AtlasEngine Instance
		{
			get
			{
				if(instance == null)
					instance = new AtlasEngine();
				return instance;
			}
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

		private Stopwatch timer = new Stopwatch();
		private bool isUpdating = false;
		private bool isRunning = false;

		private float deltaUpdateTime = 0;
		private float totalUpdateTime = 0;
		private ISystem currentUpdateSystem;

		private float deltaFixedUpdateTime = (float)1 / 60;
		private float totalFixedUpdateTime = 0;
		private ISystem currentFixedUpdateSystem;

		private float deltaEngineTime = 0;
		private float totalEngineTime = 0;

		private Signal<IEngine, IEntity> entityAdded = new Signal<IEngine, IEntity>();
		private Signal<IEngine, IEntity> entityRemoved = new Signal<IEngine, IEntity>();
		private Signal<IEngine, Type> familyAdded = new Signal<IEngine, Type>();
		private Signal<IEngine, Type> familyRemoved = new Signal<IEngine, Type>();
		private Signal<IEngine, ISystem, Type> systemAdded = new Signal<IEngine, ISystem, Type>();
		private Signal<IEngine, ISystem, Type> systemRemoved = new Signal<IEngine, ISystem, Type>();
		private Signal<IEngine, bool> isUpdatingChanged = new Signal<IEngine, bool>();

		private AtlasEngine()
		{

		}

		override protected void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			AddEntity(entity);
		}

		override protected void RemovingManager(IEntity entity, int index)
		{
			RemoveEntity(entity);
			base.RemovingManager(entity, index);
		}

		protected override void Destroying()
		{
			//Not sure about this one...
			//Null out Engine singleton to allow
			//anothjer top be instantiated.
			if(instance == this)
				instance = null;

			entityAdded.Dispose();
			entityRemoved.Dispose();
			familyAdded.Dispose();
			familyRemoved.Dispose();
			systemAdded.Dispose();
			systemRemoved.Dispose();
			isUpdatingChanged.Dispose();
			base.Destroying();
		}

		protected override void Resetting()
		{
			base.Resetting();
		}

		ISignal<IEngine, IEntity> IEngine.EntityAdded { get { return entityAdded; } }
		ISignal<IEngine, IEntity> IEngine.EntityRemoved { get { return entityRemoved; } }
		public ISignal<IEngine, Type> FamilyAdded { get { return familyAdded; } }
		public ISignal<IEngine, Type> FamilyRemoved { get { return familyRemoved; } }
		public ISignal<IEngine, ISystem, Type> SystemAdded { get { return systemAdded; } }
		public ISignal<IEngine, ISystem, Type> SystemRemoved { get { return systemRemoved; } }
		public ISignal<IEngine, bool> IsUpdatingChanged { get { return isUpdatingChanged; } }

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
				entity.StateChanged.Add(EntityStateChanged, int.MinValue);
			}
			entity.GlobalName = globalName;
			entity.LocalName = localName;
			return entity;
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
			if(entitiesGlobalName.ContainsKey(entity.GlobalName) && entitiesGlobalName[entity.GlobalName] != entity)
				entity.GlobalName = Guid.NewGuid().ToString("N");

			if(!entitiesGlobalName.ContainsKey(entity.GlobalName))
			{
				entitiesGlobalName.Add(entity.GlobalName, entity);
				entities.Add(entity);

				entity.ChildAdded.Add(EntityChildAdded, int.MinValue);
				entity.ParentChanged.Add(EntityAncestorChanged, int.MinValue);
				entity.GlobalNameChanged.Add(EntityGlobalNameChanged, int.MinValue);
				entity.ComponentAdded.Add(EntityComponentAdded, int.MinValue);
				entity.ComponentRemoved.Add(EntityComponentRemoved, int.MinValue);
				entity.SystemAdded.Add(EntitySystemAdded, int.MinValue);
				entity.SystemRemoved.Add(EntitySystemRemoved, int.MinValue);

				entity.Engine = this;

				foreach(Type type in entity.Systems)
					EntitySystemAdded(entity, type);

				foreach(IFamily family in families)
					family.AddEntity(entity);

				entityAdded.Dispatch(this, entity);

				foreach(IEntity child in entity.Children)
					AddEntity(child);
			}
		}

		private void RemoveEntity(IEntity entity)
		{
			foreach(IEntity child in entity.Children)
				RemoveEntity(child);

			entitiesGlobalName.Remove(entity.GlobalName);
			entities.Remove(entity);

			entity.ChildAdded.Remove(EntityChildAdded);
			entity.ParentChanged.Remove(EntityAncestorChanged);
			entity.GlobalNameChanged.Remove(EntityGlobalNameChanged);
			entity.ComponentAdded.Remove(EntityComponentAdded);
			entity.ComponentRemoved.Remove(EntityComponentRemoved);
			entity.SystemAdded.Remove(EntitySystemAdded);
			entity.SystemRemoved.Remove(EntitySystemRemoved);

			foreach(Type type in entity.Systems)
				EntitySystemRemoved(entity, type);

			foreach(IFamily family in families)
				family.RemoveEntity(entity);

			entity.Engine = null;

			entityRemoved.Dispatch(this, entity);
		}

		private void EntityChildAdded(IEntity parent, IEntity child, int index)
		{
			AddEntity(child);
		}

		private void EntityAncestorChanged(IEntity child, IEntity next, IEntity previous, IEntity source)
		{
			if(child != source)
				return;
			if(next != null)
				return;
			RemoveEntity(child);
		}

		private void EntityGlobalNameChanged(IEntity entity, string next, string previous)
		{
			entitiesGlobalName.Remove(previous);
			entitiesGlobalName.Add(next, entity);
		}

		private void EntityStateChanged(IEntity entity, EngineObjectState current, EngineObjectState previous)
		{
			if(current != EngineObjectState.Destroyed)
				return;
			entity.StateChanged.Remove(EntityStateChanged);
			entityPool.Push(entity);
		}

		#endregion

		#region Systems

		public float DeltaUpdateTime
		{
			get
			{
				return deltaUpdateTime;
			}
			private set
			{
				if(deltaUpdateTime == value)
					return;
				deltaUpdateTime = value;
			}
		}

		public float TotalUpdateTime
		{
			get
			{
				return totalUpdateTime;
			}
			private set
			{
				if(totalUpdateTime == value)
					return;
				totalUpdateTime = value;
			}
		}

		public float DeltaFixedUpdateTime
		{
			get
			{
				return deltaFixedUpdateTime;
			}
			set
			{
				if(deltaFixedUpdateTime == value)
					return;
				deltaFixedUpdateTime = value;
			}
		}

		public float TotalFixedUpdateTime
		{
			get
			{
				return totalFixedUpdateTime;
			}
			private set
			{
				if(totalFixedUpdateTime == value)
					return;
				totalFixedUpdateTime = value;
			}
		}

		public float DeltaEngineTime
		{
			get
			{
				return deltaEngineTime;
			}
			private set
			{
				if(deltaEngineTime == value)
					return;
				deltaEngineTime = value;
			}
		}
		public float TotalEngineTime
		{
			get
			{
				return totalEngineTime;
			}
			private set
			{
				if(totalEngineTime == value)
					return;
				totalEngineTime = value;
			}
		}


		public bool IsUpdating
		{
			get
			{
				return isUpdating;
			}
			private set
			{
				if(isUpdating == value)
					return;
				isUpdating = value;
				isUpdatingChanged.Dispatch(this, value);
			}
		}

		public ISystem CurrentFixedUpdateSystem
		{
			get
			{
				return currentFixedUpdateSystem;
			}
			private set
			{
				if(currentFixedUpdateSystem == value)
					return;
				currentFixedUpdateSystem = value;
			}
		}

		public ISystem CurrentUpdateSystem
		{
			get
			{
				return currentUpdateSystem;
			}
			private set
			{
				if(currentUpdateSystem == value)
					return;
				currentUpdateSystem = value;
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
			RemoveSystemType(type);
			systemsInstance.Add(type, instance);
			if(systemsCount.ContainsKey(type))
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
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(!systemsInstance.ContainsKey(type))
				return false;
			systemsInstance.Remove(type);
			if(systemsCount.ContainsKey(type))
				RemoveSystem(type);
			return true;
		}

		private void AddSystem(Type type)
		{
			if(systemsType.ContainsKey(type))
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
			system.PriorityChanged.Add(SystemPriorityChanged);
			SystemPriorityChanged(system, system.Priority, 0);
			system.Interface = type;
			system.Engine = this;
			systemAdded.Dispatch(this, system, type);
		}

		private void RemoveSystem(Type type)
		{
			if(!systemsType.ContainsKey(type))
				return;
			ISystem system = systemsType[type];
			system.PriorityChanged.Remove(SystemPriorityChanged);
			systems.Remove(system);
			systemsType.Remove(type);
			systemRemoved.Dispatch(this, system, type);
			system.Destroy();
		}

		private void EntitySystemAdded(IEntity entity, Type type)
		{
			if(!systemsCount.ContainsKey(type))
				systemsCount.Add(type, 0);
			++systemsCount[type];
			AddSystem(type);
		}

		private void EntitySystemRemoved(IEntity entity, Type type)
		{
			if(!systemsCount.ContainsKey(type))
				return;
			--systemsCount[type];
			if(systemsCount[type] > 0)
				return;
			systemsCount.Remove(type);
			RemoveSystem(type);
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
			if(system == null)
				return false;
			if(system.Interface == null)
				return false;
			return systemsType.ContainsKey(system.Interface) && systemsType[system.Interface] == system;
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

		public void Run()
		{
			if(!IsRunning)
			{
				IsRunning = true;
				while(true)
				{
					Update();
				}
				//IsRunning = false;
			}
		}

		private void Update()
		{
			IsUpdating = true;

			//timer.Start();

			DeltaEngineTime = (float)timer.Elapsed.TotalSeconds - TotalEngineTime;

			//DeltaFixedUpdateTime can be changed, but we probably shouldn't change it during an update loop.
			float deltaFixedUpdateTime = this.deltaFixedUpdateTime;
			while(TotalFixedUpdateTime < timer.Elapsed.TotalSeconds)
			{
				foreach(ISystem system in systems)
				{
					CurrentFixedUpdateSystem = system;
					system.FixedUpdate(deltaFixedUpdateTime);
					CurrentFixedUpdateSystem = null;
				}
				TotalFixedUpdateTime += deltaFixedUpdateTime;
			}

			DeltaUpdateTime = (float)timer.Elapsed.TotalSeconds - totalUpdateTime;
			foreach(ISystem system in systems)
			{
				CurrentUpdateSystem = system;
				system.Update(deltaUpdateTime);
				CurrentUpdateSystem = null;
			}
			TotalUpdateTime = (float)timer.Elapsed.TotalSeconds;

			DisposeSystems();
			DisposeFamilies();

			TotalEngineTime = (float)timer.Elapsed.TotalSeconds;

			IsUpdating = false;
		}

		private void DisposeSystems()
		{
			while(systemsRemoved.Count > 0)
			{
				ISystem system = systemsRemoved[0];
				systemsRemoved.RemoveAt(0);
				DestroySystem(system);
			}
		}

		private void DestroySystem(ISystem system)
		{
			Type type = system.GetType();
			system.PriorityChanged.Remove(SystemPriorityChanged);
			systems.Remove(system);
			systemsType.Remove(type);
			systemsCount.Remove(type);
			systemRemoved.Dispatch(this, system, type);
			system.Destroy();
		}

		public bool IsRunning
		{
			get
			{
				return isRunning;
			}
			private set
			{
				if(isRunning == value)
					return;
				isRunning = value;
				if(isRunning)
				{
					timer.Start();
					DeltaUpdateTime = 0;
					TotalUpdateTime = 0;
					TotalFixedUpdateTime = 0;
					DeltaEngineTime = 0;
					TotalEngineTime = 0;
				}
				else
				{
					timer.Reset();
					//Time resets could go here too.
				}
			}
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
			IFamily family;

			if(!familiesType.ContainsKey(type))
			{
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

				foreach(IEntity entity in entities)
				{
					family.AddEntity(entity);
				}

				familyAdded.Dispatch(this, type);
			}
			else
			{
				family = familiesType[type];
				//Family was marked for removal, but was requested again during update.
				if(familiesCount[type] == 0)
					familiesRemoved.Remove(family);
				++familiesCount[type];
			}
			return family;
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

			if(familiesCount[type] > 0)
			{
				if(--familiesCount[type] == 0)
				{
					if(IsUpdating)
					{
						familiesRemoved.Add(family);
					}
					else
					{
						DestroyFamily(family);
					}
				}
			}

			return family;
		}

		private void DisposeFamilies()
		{
			while(familiesRemoved.Count > 0)
			{
				IFamily family = familiesRemoved[0];
				familiesRemoved.RemoveAt(0);
				DestroyFamily(family);
			}
		}

		private void DestroyFamily(IFamily family)
		{
			Type type = family.FamilyType;
			families.Remove(family);
			familiesType.Remove(type);
			familiesCount.Remove(type);
			familyRemoved.Dispatch(this, type);
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

		private void EntityComponentAdded(IEntity entity, IComponent component, Type componentType, IEntity source)
		{
			if(entity != source)
				return;
			foreach(IFamily family in families)
			{
				family.AddEntity(entity, componentType);
			}
		}

		private void EntityComponentRemoved(IEntity entity, IComponent component, Type componentType, IEntity source)
		{
			if(entity != source)
				return;
			foreach(IFamily family in families)
			{
				family.RemoveEntity(entity, componentType);
			}
		}

		#endregion
	}
}