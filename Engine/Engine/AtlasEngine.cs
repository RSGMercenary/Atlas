using Atlas.Engine.Collections.Fixed;
using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas.Engine.Engine
{
	sealed class AtlasEngine:AtlasComponent<IEngine>, IEngine
	{
		private static AtlasEngine instance;

		private Type defaultEntity = AtlasEngineDefaults.DefaultEntity;
		private Type defaultFamily = AtlasEngineDefaults.DefaultFamily;

		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private LinkList<IFamily> families = new LinkList<IFamily>();
		private LinkList<ISystem> systems = new LinkList<ISystem>();

		private Dictionary<string, IEntity> entitiesGlobalName = new Dictionary<string, IEntity>();
		private Dictionary<Type, IFamily> familiesType = new Dictionary<Type, IFamily>();
		private Dictionary<Type, ISystem> systemsType = new Dictionary<Type, ISystem>();

		private FixedStack<IEntity> entityPool = new FixedStack<IEntity>(AtlasEngineDefaults.DefaultEntityPoolCapacity);
		private FixedStack<IFamily> familyPool = new FixedStack<IFamily>(AtlasEngineDefaults.DefaultFamilyPoolCapacity);

		private Dictionary<Type, int> familyCounts = new Dictionary<Type, int>();
		private Dictionary<Type, int> systemCounts = new Dictionary<Type, int>();

		private Stack<IFamily> familiesRemoved = new Stack<IFamily>();
		private Stack<ISystem> systemsRemoved = new Stack<ISystem>();

		private ISystem currentSystem;
		private int sleeping = 0;
		private bool isUpdating = false;

		private Signal<IEngine, IEntity> entityAdded = new Signal<IEngine, IEntity>();
		private Signal<IEngine, IEntity> entityRemoved = new Signal<IEngine, IEntity>();
		private Signal<IEngine, Type> familyAdded = new Signal<IEngine, Type>();
		private Signal<IEngine, Type> familyRemoved = new Signal<IEngine, Type>();
		private Signal<IEngine, Type> systemAdded = new Signal<IEngine, Type>();
		private Signal<IEngine, Type> systemRemoved = new Signal<IEngine, Type>();
		private Signal<IEngine, int, int> sleepingChanged = new Signal<IEngine, int, int>();
		private Signal<IEngine, bool> isUpdatingChanged = new Signal<IEngine, bool>();

		//private int _frameRate = 60;
		//private int _maxUpdates = 5;
		//private double _timeTotal = 0; //TO-DO :: Not sure if this should be float, double, or...
		//private double _timeElapsedMax = 1;
		//private double _timePrevious;

		private AtlasEngine()
		{

		}

		public static AtlasEngine Instance
		{
			get
			{
				if(instance == null)
					instance = new AtlasEngine();
				return instance;
			}
		}

		public FixedStack<IEntity> EntityPool { get { return entityPool; } }
		public FixedStack<IFamily> FamilyPool { get { return familyPool; } }

		public IReadOnlyLinkList<IEntity> Entities { get { return entities; } }
		public IReadOnlyLinkList<IFamily> Families { get { return families; } }
		public IReadOnlyLinkList<ISystem> Systems { get { return systems; } }

		public ISignal<IEngine, IEntity> EntityAdded { get { return entityAdded; } }
		public ISignal<IEngine, IEntity> EntityRemoved { get { return entityRemoved; } }
		public ISignal<IEngine, Type> FamilyAdded { get { return familyAdded; } }
		public ISignal<IEngine, Type> FamilyRemoved { get { return familyRemoved; } }
		public ISignal<IEngine, Type> SystemAdded { get { return systemAdded; } }
		public ISignal<IEngine, Type> SystemRemoved { get { return systemRemoved; } }
		public ISignal<IEngine, int, int> SleepingChanged { get { return sleepingChanged; } }
		public ISignal<IEngine, bool> IsUpdatingChanged { get { return isUpdatingChanged; } }

		override protected void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			AddEntity(entity);
			entity.ParentChanged.Remove(EntityParentChanged);
			entity.Parent = null;
		}

		override protected void RemovingManager(IEntity entity, int index)
		{
			RemoveEntity(entity);
			base.RemovingManager(entity, index);
		}

		#region Entities

		public Type DefaultEntity
		{
			get
			{
				return defaultEntity;
			}
			set
			{
				if(defaultEntity == value)
					return;
				defaultEntity = value;
				entityPool.Clear();
			}
		}

		public IEntity GetEntity()
		{
			IEntity entity;
			if(entityPool.Count > 0)
			{
				entity = entityPool.Pop();
			}
			else
			{
				try
				{
					entity = Activator.CreateInstance(defaultEntity) as IEntity;
				}
				catch(Exception e)
				{
					Debug.WriteLine(e);
					return null;
				}
			}
			entity.IsDisposedChanged.Add(EntityDisposed, int.MinValue);
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
			{
				entity.GlobalName = new Guid().ToString("N");
			}
			if(!entitiesGlobalName.ContainsKey(entity.GlobalName))
			{
				entitiesGlobalName.Add(entity.GlobalName, entity);
				entities.Add(entity);

				entity.ChildAdded.Add(EntityChildAdded, int.MinValue);
				entity.ParentChanged.Add(EntityParentChanged, int.MinValue);
				entity.GlobalNameChanged.Add(EntityGlobalNameChanged, int.MinValue);
				entity.ComponentAdded.Add(EntityComponentAdded, int.MinValue);
				entity.ComponentRemoved.Add(EntityComponentRemoved, int.MinValue);
				entity.SystemAdded.Add(EntitySystemAdded, int.MinValue);
				entity.SystemRemoved.Add(EntitySystemRemoved, int.MinValue);

				entity.Engine = this;

				foreach(Type type in entity.Systems)
				{
					EntitySystemAdded(entity, type);
				}

				for(ILinkListNode<IFamily> current = families.First; current != null;)
				{
					current.Value.AddEntity(entity);
					current = current.Next;
				}

				entityAdded.Dispatch(this, entity);

				for(ILinkListNode<IEntity> current = entity.Children.First; current != null;)
				{
					AddEntity(current.Value);
					current = current.Next;
				}
			}
		}

		private void RemoveEntity(IEntity entity)
		{
			for(ILinkListNode<IEntity> current = entity.Children.First; current != null;)
			{
				RemoveEntity(current.Value);
				current = current.Next;
			}

			entityRemoved.Dispatch(this, entity);

			entitiesGlobalName.Remove(entity.GlobalName);
			entities.Remove(entity);

			entity.ChildAdded.Remove(EntityChildAdded);
			entity.ParentChanged.Remove(EntityParentChanged);
			entity.GlobalNameChanged.Remove(EntityGlobalNameChanged);
			entity.ComponentAdded.Remove(EntityComponentAdded);
			entity.ComponentRemoved.Remove(EntityComponentRemoved);
			entity.SystemAdded.Remove(EntitySystemAdded);
			entity.SystemRemoved.Remove(EntitySystemRemoved);

			foreach(Type type in entity.Systems)
			{
				EntitySystemRemoved(entity, type);
			}

			for(ILinkListNode<IFamily> current = families.First; current != null;)
			{
				current.Value.RemoveEntity(entity);
				current = current.Next;
			}

			entity.Engine = null;
		}

		private void EntityChildAdded(IEntity parent, IEntity child, int index)
		{
			AddEntity(child);
		}

		private void EntityParentChanged(IEntity child, IEntity next, IEntity previous)
		{
			if(next != null)
				return;
			RemoveEntity(child);
		}

		private void EntityGlobalNameChanged(IEntity entity, string next, string previous)
		{
			entitiesGlobalName.Remove(previous);
			entitiesGlobalName.Add(next, entity);
		}

		private void EntityDisposed(IEntity entity, bool isDisposed)
		{
			if(isDisposed)
			{
				entity.IsDisposedChanged.Remove(EntityDisposed);
				if(defaultEntity.IsInstanceOfType(entity))
					entityPool.Push(entity);
			}
		}

		#endregion

		#region Systems

		private void EntitySystemAdded(IEntity entity, Type type)
		{
			if(!systemsType.ContainsKey(type))
			{
				ISystem system;
				try
				{
					system = Activator.CreateInstance(type) as ISystem;
				}
				catch(Exception e)
				{
					Debug.WriteLine(e);
					return;
				}
				systemsType.Add(type, system);
				systemCounts.Add(type, 1);
				system.PriorityChanged.Add(SystemPriorityChanged);
				SystemPriorityChanged(system, system.Priority, 0);

				system.Engine = this;

				systemAdded.Dispatch(this, type);
			}
			else
			{
				++systemCounts[type];
			}
		}

		private void EntitySystemRemoved(IEntity entity, Type type)
		{
			if(!systemsType.ContainsKey(type))
				return;

			if(--systemCounts[type] <= 0)
			{
				ISystem system = systemsType[type];
				systemRemoved.Dispatch(this, type);

				system.PriorityChanged.Remove(SystemPriorityChanged);

				systemsType.Remove(type);
				systemCounts.Remove(type);
				systems.Remove(system);

				if(isUpdating)
				{
					systemsRemoved.Push(system);
				}
				else
				{
					system.Dispose();
				}
			}
		}

		private void SystemPriorityChanged(ISystem system, int current, int previous)
		{
			systems.Remove(system);

			for(int index = systems.Count; index > 0; --index)
			{
				if(systems[index - 1].Priority <= current)
				{
					systems.Add(system, index);
					return;
				}
			}

			systems.Add(system, 0);
		}

		public bool HasSystem(ISystem system)
		{
			return systemsType.ContainsKey(system.GetType()) && systemsType[system.GetType()] == system;
		}

		public bool HasSystem<TSystem>() where TSystem : ISystem
		{
			return HasSystem(typeof(TSystem));
		}

		public bool HasSystem(Type systemType)
		{
			return systemsType.ContainsKey(systemType);
		}

		public TSystem GetSystem<TSystem>() where TSystem : ISystem
		{
			return (TSystem)GetSystem(typeof(TSystem));
		}

		public ISystem GetSystem(Type type)
		{
			return systemsType.ContainsKey(type) ? systemsType[type] : null;
		}

		public ISystem GetSystem(int index)
		{
			return systems[index];
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
				bool previous = isUpdating;
				isUpdating = value;
				isUpdatingChanged.Dispatch(this, value);
			}
		}

		public void Update()
		{
			if(IsSleeping)
				return;

			if(!IsUpdating)
			{
				IsUpdating = true;

				ILinkListNode<ISystem> current = systems.First;
				while(current != null)
				{
					currentSystem = current.Value;
					current.Value.Update();
					current = current.Next;
					currentSystem = null;
				}

				IsUpdating = false;

				DisposeSystems();
				DisposeFamilies();
			}
		}

		private void DisposeSystems()
		{
			while(systemsRemoved.Count > 0)
			{
				systemsRemoved.Pop().Dispose();
			}
		}

		public ISystem CurrentSystem
		{
			get
			{
				return currentSystem;
			}
		}

		public bool IsSleeping
		{
			get
			{
				return sleeping > 0;
			}
		}

		public int Sleeping
		{
			get
			{
				return sleeping;
			}
			set
			{
				if(sleeping == value)
					return;
				int previous = sleeping;
				sleeping = value;
				sleepingChanged.Dispatch(this, value, previous);
			}
		}

		/*private function onEnterFrame(event:Event):Void
		{
			var timeCurrent:Float 	= getTimer();
			var timeElapsed:Float 	= (timeCurrent - this._timePrevious) / 1000;
			this._timePrevious 		= timeCurrent;

			if(timeElapsed > this._timeElapsedMax)
			{
				timeElapsed = this._timeElapsedMax;
			}

			this._timeTotal += timeElapsed;

			var frameTime:Float = 1 / this._frameRate;
			var numUpdates:UInt = Math.floor(this._timeTotal / frameTime);

			this._timeTotal -= numUpdates * frameTime;

			if(numUpdates > this._maxUpdates)
			{
				numUpdates = this._maxUpdates;
			}

			for(index in 0...numUpdates - 1)
			{
				this.update(frameTime);
			}
		}*/

		#endregion

		#region Families

		public Type DefaultFamily
		{
			get
			{
				return defaultFamily;
			}
			set
			{
				if(defaultFamily == value)
					return;
				defaultFamily = value;
				familyPool.Clear();
			}
		}

		public bool HasFamily(IFamily family)
		{
			return familiesType.ContainsKey(family.FamilyType.GetType()) && familiesType[family.FamilyType.GetType()] == family;
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
					try
					{
						family = Activator.CreateInstance(defaultFamily) as IFamily;
					}
					catch(Exception e)
					{
						Debug.WriteLine(e);
						return null;
					}
				}

				families.Add(family);
				familiesType.Add(type, family);
				familyCounts.Add(type, 1);
				family.FamilyType = type;
				family.Engine = this;

				ILinkListNode<IEntity> current = entities.First;
				while(current != null)
				{
					family.AddEntity(current.Value);
					current = current.Next;
				}

				++familyCounts[type];

				familyAdded.Dispatch(this, type);
			}
			else
			{
				family = familiesType[type];
				++familyCounts[type];
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

			if(--familyCounts[type] == 0)
			{
				familyRemoved.Dispatch(this, type);
				familiesType.Remove(type);
				familyCounts.Remove(type);

				family.Engine = null;

				if(isUpdating)
				{
					familiesRemoved.Push(family);
				}
				else
				{
					DisposeFamily(family);
				}
			}

			return family;
		}

		private void DisposeFamilies()
		{
			while(familiesRemoved.Count > 0)
			{
				DisposeFamily(familiesRemoved.Pop());
			}
		}

		private void DisposeFamily(IFamily family)
		{
			family.Dispose();
			if(defaultFamily.IsInstanceOfType(family))
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

		private void EntityComponentAdded(IEntity entity, IComponent component, Type componentType)
		{
			ILinkListNode<IFamily> current = families.First;
			while(current != null)
			{
				current.Value.AddEntity(entity, component, componentType);
				current = current.Next;
			}
		}

		private void EntityComponentRemoved(IEntity entity, IComponent component, Type componentType)
		{
			ILinkListNode<IFamily> current = families.First;
			while(current != null)
			{
				current.Value.RemoveEntity(entity, component, componentType);
				current = current.Next;
			}
		}

		#endregion
	}
}
