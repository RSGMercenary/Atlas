using Atlas.Components;
using Atlas.Entities;
using Atlas.Nodes;
using Atlas.Signals;
using Atlas.Systems;
using System;
using System.Collections.Generic;

namespace Atlas
{
	class Atlas:Component
	{
		//Entities
		private List<Entity> entities = new List<Entity>();
		private Dictionary<string, Entity> uniqueNames = new Dictionary<string, Entity>();
		private Signal<Atlas, Entity> entityAdded = new Signal<Atlas, Entity>();
		private Signal<Atlas, Entity> entityRemoved = new Signal<Atlas, Entity>();

		//Systems
		private List<AtlasSystem> systems = new List<AtlasSystem>();
		private Dictionary<Type, AtlasSystem> systemTypes = new Dictionary<Type, AtlasSystem>();
		private Signal<SystemManager, Type> systemAdded = new Signal<SystemManager, Type>();
		private Signal<SystemManager, Type> systemRemoved = new Signal<SystemManager, Type>();
		private List<AtlasSystem> systemsRemoved = new List<AtlasSystem>();

		private int sleeping = 1;
		private Signal<SystemManager, int> sleepingChanged = new Signal<SystemManager, int>();

		private bool isUpdating = false;
		private Signal<SystemManager, bool> isUpdatingChanged = new Signal<SystemManager, bool>();

		private int _frameRate = 60;
		private int _maxUpdates = 5;
		private double _timeTotal = 0; //TO-DO :: Not sure if this should be float, double, or...
		private double _timeElapsedMax = 1;
		private double _timePrevious;

		//Nodes
		private NodeList first;
		private NodeList last;

		private Dictionary<Type, NodeList> nodeTypes = new Dictionary<Type, NodeList>();
		private List<NodeList> nodeListsPooled = new List<NodeList>();
		private List<NodeList> nodeListsRemoved = new List<NodeList>();
		private Signal<Atlas, Type> nodeListAdded = new Signal<Atlas, Type>();
		private Signal<Atlas, Type> nodeListRemoved = new Signal<Atlas, Type>();

		public Atlas()
		{

		}

		override protected void AddingComponentManager(Entity entity)
		{
			base.AddingComponentManager(entity);
			AddEntity(entity);
		}

		override protected void RemovingComponentManager(Entity entity)
		{
			RemoveEntity(entity);
			base.RemovingComponentManager(entity);
		}

		public bool IsUniqueName(string uniqueName)
		{
			return !string.IsNullOrWhiteSpace(uniqueName) && !uniqueNames.ContainsKey(uniqueName);
		}

		public bool HasEntity(Entity entity)
		{
			return entity != null && entities.Contains(entity);
		}

		public string GetUniqueName()
		{
			for(int index = 0; index < int.MaxValue; ++index)
			{
				if(!uniqueNames.ContainsKey("instance" + index))
				{
					return "instance" + index;
				}
			}
			return "";
		}

		public Signal<Atlas, Entity> EntityAdded
		{
			get
			{
				return entityAdded;
			}
		}

		public Signal<Atlas, Entity> EntityRemoved
		{
			get
			{
				return entityRemoved;
			}
		}

		public Entity GetEntityAt(int index)
		{
			if(index < 0)
				return null;
			if(index > entities.Count - 1)
				return null;
			return entities[index];
		}

		public int GetEntityIndex(Entity entity)
		{
			return entities.IndexOf(entity);
		}

		public bool SetEntityIndex(Entity entity, int index)
		{
			int previous = entities.IndexOf(entity);

			if(previous == index)
				return true;
			if(previous < 0)
				return false;

			index = Math.Max(0, Math.Min(index, entities.Count - 1));

			entities.RemoveAt(previous);
			entities.Insert(index, entity);
			return true;
		}

		public int NumEntities
		{
			get
			{
				return entities.Count;
			}
		}

		public Entity GetEntityByUniqueName(string uniqueName)
		{
			return uniqueNames[uniqueName];
		}

		public List<Entity> Entities
		{
			get
			{
				return new List<Entity>(entities);
			}
		}

		private void AddEntity(Entity entity)
		{
			if(string.IsNullOrWhiteSpace(entity.UniqueName) || (uniqueNames.ContainsKey(entity.UniqueName) && uniqueNames[entity.UniqueName] != entity))
			{
				entity.UniqueName = GetUniqueName();
			}
			if(!uniqueNames.ContainsKey(entity.UniqueName))
			{
				uniqueNames.Add(entity.UniqueName, entity);
				entities.Add(entity);

				entity.ChildAdded.Add(ChildAdded, int.MinValue);
				entity.ParentChanged.Add(ParentChanged, int.MinValue);
				entity.UniqueNameChanged.Add(UniqueNameChanged, int.MinValue);
				entity.ComponentAdded.Add(ComponentAdded, int.MinValue);
				entity.ComponentRemoved.Add(ComponentRemoved, int.MinValue);
				entity.SystemTypeAdded.Add(SystemTypeAdded, int.MinValue);
				entity.SystemTypeRemoved.Add(SystemTypeRemoved, int.MinValue);

				entity.Atlas = this;

				foreach(Type systemType in entity.SystemTypes)
				{
					SystemTypeAdded(entity, systemType);
				}

				for(NodeList current = first; current; current = current.next)
				{
					current.EntityAdded(entity);
				}

				entityAdded.Dispatch(this, entity);

				if(entity.NumChildren > 0)
				{
					foreach(Entity child in entity.Children)
					{
						AddEntity(child);
					}
				}
			}
		}

		private void RemoveEntity(Entity entity)
		{
			if(entity.NumChildren > 0)
			{
				foreach(Entity child in entity.Children)
				{
					RemoveEntity(child);
				}
			}

			entityRemoved.Dispatch(this, entity);

			uniqueNames.Remove(entity.UniqueName);
			entities.Remove(entity);

			foreach(Type systemType in entity.SystemTypes)
			{
				SystemTypeRemoved(entity, systemType);
			}

			for(NodeList current = first; current; current = current.next)
			{
				current.EntityRemoved(entity);
			}

			entity.ChildAdded.Remove(ChildAdded);
			entity.ParentChanged.Remove(ParentChanged);
			entity.UniqueNameChanged.Remove(UniqueNameChanged);
			entity.ComponentAdded.Remove(ComponentAdded);
			entity.ComponentRemoved.Remove(ComponentRemoved);
			entity.SystemTypeAdded.Remove(SystemTypeAdded);
			entity.SystemTypeRemoved.Remove(SystemTypeRemoved);

			entity.Atlas = null;
		}

		private void ChildAdded(Entity parent, Entity child, int index)
		{
			AddEntity(child);
		}

		private void ParentChanged(Entity child, Entity next, Entity previous)
		{
			if(next == null)
			{
				RemoveEntity(child);
			}
		}

		private void UniqueNameChanged(Entity entity, string next, string previous)
		{
			//This should account for a possible name change in the middle of a name change.
			if(uniqueNames.ContainsKey(previous) && uniqueNames[previous] == entity)
			{
				uniqueNames.Remove(previous);
			}
			if(!uniqueNames.ContainsKey(next) && entity.UniqueName == next)
			{
				uniqueNames.Add(next, entity);
			}
		}

		private void ComponentAdded(Entity entity, Component component, Type componentType)
		{
			for(NodeList current = first; current; current = current.next)
			{
				current.ComponentAdded(entity, componentType);
			}
		}

		private void ComponentRemoved(Entity entity, Component component, Type componentType)
		{
			for(NodeList current = first; current; current = current.next)
			{
				current.ComponentRemoved(entity, componentType);
			}
		}

		private void SystemTypeAdded(Entity entity, Type systemType)
		{
			if(!systemTypes.ContainsKey(systemType))
			{
				AtlasSystem system = Activator.CreateInstance(systemType) as AtlasSystem;

				systemTypes.Add(systemType, system);
				system.AddSystemManager(entity);

				system.SleepingChanged.Add(SystemSleepingChanged, int.MinValue);
				if(!system.IsSleeping)
				{
					SystemSleepingChanged(system, system.Sleeping, 1);
				}

				system.Atlas = this;

				systemAdded.Dispatch(this, systemType);
			}
			else
			{
				systemTypes[systemType].AddSystemManager(entity);
			}
		}

		private void SystemTypeRemoved(Entity entity, Type systemType)
		{
			AtlasSystem system = systemTypes[systemType];

			if(system != null)
			{
				system.RemoveSystemManager(entity);

				if(system.NumSystemManagers == 0)
				{
					systemRemoved.Dispatch(this, systemType);

					system.SleepingChanged.Remove(SystemSleepingChanged);
					if(!system.IsSleeping)
					{
						SystemSleepingChanged(system, 1, 0);
					}

					systemTypes.Remove(systemType);

					if(isUpdating)
					{
						systemsRemoved.Add(system);
					}
					else
					{
						system.Dispose();
					}
				}
			}
		}

		private void SystemSleepingChanged(AtlasSystem system, int current, int previous)
		{
			if(current <= 0 && previous > 0)
			{
				system.PriorityChanged.Add(SystemPriorityChanged, int.MinValue);
				SystemPriorityChanged(system, current, 0);
			}
			else if(current > 0 && previous <= 0)
			{
				system.PriorityChanged.Remove(SystemPriorityChanged);
				systems.Remove(system);
			}
		}

		private void SystemPriorityChanged(AtlasSystem system, int current, int previous)
		{
			systems.Remove(system);

			for(int index = systems.Count; index > 0; --index)
			{
				if(systems[index - 1].Priority <= system.Priority)
				{
					systems.Insert(index, system);
					return;
				}
			}

			systems.Insert(0, system);
		}

		public Signal<SystemManager, Type> SystemAdded
		{
			get
			{
				return systemAdded;
			}
		}

		public Signal<SystemManager, Type> SystemRemoved
		{
			get
			{
				return systemRemoved;
			}
		}

		public bool HasSystem<T>() where T : AtlasSystem
		{
			return HasSystem(typeof(T));
		}

		public bool HasSystem(Type systemType)
		{
			return systemTypes.ContainsKey(systemType);
		}

		public AtlasSystem GetSystemByType(Type systemType)
		{
			return systemTypes.ContainsKey(systemType) ? systemTypes[systemType] : null;
		}

		public AtlasSystem GetSystemAt(int index)
		{
			if(index < 0)
				return null;
			if(index > systems.Count - 1)
				return null;
			return systems[index];
		}

		public int GetSystemIndex(AtlasSystem system)
		{
			return systems.IndexOf(system);
		}

		public List<AtlasSystem> Systems
		{
			get
			{
				return new List<AtlasSystem>(systems);
			}
		}

		public List<Type> SystemTypes
		{
			get
			{
				return new List<Type>(systemTypes.Keys);
			}
		}

		public int NumSystems
		{
			get
			{
				return systems.Count;
			}
		}

		public bool IsUpdating
		{
			get
			{
				return isUpdating;
			}
			internal set
			{
				if(isUpdating != value)
				{
					bool previous = isUpdating;
					isUpdating = value;
					isUpdatingChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<SystemManager, bool> IsUpdatingChanged
		{
			get
			{
				return isUpdatingChanged;
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

					if(value <= 0 && previous > 0)
					{
						//this._timePrevious = getTimer();
						//this._updater.addEventListener(Event.ENTER_FRAME, this.onEnterFrame);
					}
					else if(value > 0 && previous <= 0)
					{
						//this._updater.removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);
					}

					sleepingChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<SystemManager, int> SleepingChanged
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

		private void Update()
		{
			IsUpdating = true;

			if(systems.Count > 0)
			{
				List<AtlasSystem> updateSystems = new List<AtlasSystem>(systems);
				foreach(AtlasSystem system in updateSystems)
				{
					system.Update();
				}
			}

			while(systemsRemoved.Count > 0)
			{
				AtlasSystem system = systemsRemoved[systemsRemoved.Count - 1];
				systemsRemoved.RemoveAt(systemsRemoved.Count - 1);
				system.Dispose();
			}

			IsUpdating = false;
		}
	}
}
