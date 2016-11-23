using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Systems;
using Atlas.Families;
using Atlas.LinkList;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Engine
{
	sealed class AtlasEngineManager:AtlasComponent<AtlasEngineManager>, IEngineManager
	{
		private static AtlasEngineManager instance;

		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private Dictionary<string, IEntity> entityGlobalNames = new Dictionary<string, IEntity>();
		private ISignal<IEngineManager, IEntity> entityAdded = new Signal<IEngineManager, IEntity>();
		private ISignal<IEngineManager, IEntity> entityRemoved = new Signal<IEngineManager, IEntity>();

		private LinkList<ISystem> systems = new LinkList<ISystem>();
		private Dictionary<Type, ISystem> systemTypes = new Dictionary<Type, ISystem>();
		private Dictionary<Type, int> systemCounts = new Dictionary<Type, int>();
		private ISignal<IEngineManager, Type> systemAdded = new Signal<IEngineManager, Type>();
		private ISignal<IEngineManager, Type> systemRemoved = new Signal<IEngineManager, Type>();
		private Stack<ISystem> systemsRemoved = new Stack<ISystem>();
		private ISystem currentSystem;

		private LinkList<IFamily> families = new LinkList<IFamily>();
		private Dictionary<Type, IFamily> familyTypes = new Dictionary<Type, IFamily>();
		private Dictionary<Type, int> familyCounts = new Dictionary<Type, int>();
		private Stack<IFamily> familiesPooled = new Stack<IFamily>();
		private Stack<IFamily> familiesRemoved = new Stack<IFamily>();
		private ISignal<IEngineManager, Type> familyAdded = new Signal<IEngineManager, Type>();
		private ISignal<IEngineManager, Type> familyRemoved = new Signal<IEngineManager, Type>();

		private int sleeping = 0;
		private Signal<IEngineManager, int, int> sleepingChanged = new Signal<IEngineManager, int, int>();

		private bool isUpdating = false;
		private Signal<IEngineManager, bool, bool> isUpdatingChanged = new Signal<IEngineManager, bool, bool>();

		//private int _frameRate = 60;
		//private int _maxUpdates = 5;
		//private double _timeTotal = 0; //TO-DO :: Not sure if this should be float, double, or...
		//private double _timeElapsedMax = 1;
		//private double _timePrevious;

		private AtlasEngineManager()
		{

		}

		public static AtlasEngineManager Instance
		{
			get
			{
				if(instance == null)
					instance = new AtlasEngineManager();
				return instance;
			}
		}

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

		public IReadOnlyLinkList<IEntity> Entities { get { return entities; } }
		public ISignal<IEngineManager, IEntity> EntityAdded { get { return entityAdded; } }
		public ISignal<IEngineManager, IEntity> EntityRemoved { get { return entityRemoved; } }

		public bool HasEntity(string globalName)
		{
			return !string.IsNullOrWhiteSpace(globalName) && entityGlobalNames.ContainsKey(globalName);
		}

		public bool HasEntity(IEntity entity)
		{
			return entity != null && entityGlobalNames.ContainsKey(entity.GlobalName) && entityGlobalNames[entity.GlobalName] == entity;
		}

		public IEntity GetEntity(string globalName)
		{
			return entityGlobalNames.ContainsKey(globalName) ? entityGlobalNames[globalName] : null;
		}

		private void AddEntity(IEntity entity)
		{
			if(entityGlobalNames.ContainsKey(entity.GlobalName) && entityGlobalNames[entity.GlobalName] != entity)
			{
				entity.GlobalName = Guid.NewGuid().ToString("N");
			}
			if(!entityGlobalNames.ContainsKey(entity.GlobalName))
			{
				entityGlobalNames.Add(entity.GlobalName, entity);
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

			entityGlobalNames.Remove(entity.GlobalName);
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
			entityGlobalNames.Remove(previous);
			entityGlobalNames.Add(next, entity);
		}

		#endregion

		#region Systems

		public IReadOnlyLinkList<ISystem> Systems { get { return systems; } }
		public ISignal<IEngineManager, Type> SystemAdded { get { return systemAdded; } }
		public ISignal<IEngineManager, Type> SystemRemoved { get { return systemRemoved; } }

		private void EntitySystemAdded(IEntity entity, Type type)
		{
			if(!systemTypes.ContainsKey(type))
			{
				ISystem system = Activator.CreateInstance(type) as ISystem;

				systemTypes.Add(type, system);
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
			if(!systemTypes.ContainsKey(type))
				return;

			if(--systemCounts[type] <= 0)
			{
				ISystem system = systemTypes[type];
				systemRemoved.Dispatch(this, type);

				system.PriorityChanged.Remove(SystemPriorityChanged);

				systemTypes.Remove(type);
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
			return systemTypes.ContainsKey(system.GetType()) && systemTypes[system.GetType()] == system;
		}

		public bool HasSystem<T>() where T : ISystem
		{
			return HasSystem(typeof(T));
		}

		public bool HasSystem(Type systemType)
		{
			return systemTypes.ContainsKey(systemType);
		}

		public T GetSystem<T>() where T : ISystem
		{
			return (T)GetSystem(typeof(T));
		}

		public ISystem GetSystem(Type type)
		{
			return systemTypes.ContainsKey(type) ? systemTypes[type] : null;
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
				if(isUpdating != value)
				{
					bool previous = isUpdating;
					isUpdating = value;
					isUpdatingChanged.Dispatch(this, value, previous);
				}
			}
		}

		public Signal<IEngineManager, bool, bool> IsUpdatingChanged
		{
			get
			{
				return isUpdatingChanged;
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
				if(sleeping != value)
				{
					int previous = sleeping;
					sleeping = value;
					sleepingChanged.Dispatch(this, value, previous);
				}
			}
		}

		public Signal<IEngineManager, int, int> SleepingChanged
		{
			get
			{
				return sleepingChanged;
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

		public IReadOnlyLinkList<IFamily> Families { get { return families; } }

		public bool HasFamily(IFamily family)
		{
			return familyTypes.ContainsKey(family.FamilyType.GetType()) && familyTypes[family.FamilyType.GetType()] == family;
		}

		public bool HasFamily<TType>() where TType : IFamilyType
		{
			return HasFamily(typeof(TType));
		}

		public bool HasFamily(Type type)
		{
			return familyTypes.ContainsKey(type);
		}

		public IFamily AddFamily<TType>() where TType : IFamilyType
		{
			return AddFamily(typeof(TType));
		}

		public IFamily AddFamily(Type type)
		{
			AtlasFamily family;

			if(!familyTypes.ContainsKey(type))
			{
				if(familiesPooled.Count > 0)
				{
					family = familiesPooled.Pop() as AtlasFamily;
				}
				else
				{
					family = new AtlasFamily();
				}

				families.Add(family);
				familyTypes.Add(type, family);
				familyCounts.Add(type, 1);
				family.FamilyType = Activator.CreateInstance(type) as IFamilyType;
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
				family = familyTypes[type] as AtlasFamily;
				++familyCounts[type];
			}
			return family;
		}

		public IFamily RemoveFamily<TType>() where TType : IFamilyType
		{
			return RemoveFamily(typeof(TType));
		}

		public IFamily RemoveFamily(Type type)
		{
			if(!familyTypes.ContainsKey(type))
				return null;

			AtlasFamily family = familyTypes[type] as AtlasFamily;

			if(--familyCounts[type] == 0)
			{
				familyRemoved.Dispatch(this, type);
				familyTypes.Remove(type);
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
			familiesPooled.Push(family);
		}

		public IFamily GetFamily<TType>() where TType : IFamilyType
		{
			return GetFamily(typeof(TType));
		}

		public IFamily GetFamily(Type type)
		{
			return familyTypes.ContainsKey(type) ? familyTypes[type] : null;
		}

		public ISignal<IEngineManager, Type> FamilyAdded
		{
			get
			{
				return familyAdded;
			}
		}

		public ISignal<IEngineManager, Type> FamilyRemoved
		{
			get
			{
				return familyRemoved;
			}
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
