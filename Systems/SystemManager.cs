using Atlas.Components;
using Atlas.Entities;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Systems
{
	sealed class SystemManager:Component
	{
		private static SystemManager instance;

		private List<AtlasSystem> systems = new List<AtlasSystem>();
		private Dictionary<Type, AtlasSystem> systemTypes = new Dictionary<Type, AtlasSystem>();
		private Signal<SystemManager, Type> systemAdded = new Signal<SystemManager, Type>();
		private Signal<SystemManager, Type> systemRemoved = new Signal<SystemManager, Type>();
		private List<AtlasSystem> systemsRemoved = new List<AtlasSystem>();

		private int totalSleeping = 1;
		private Signal<SystemManager, int> totalSleepingChanged = new Signal<SystemManager, int>();

		private bool isUpdating = false;
		private Signal<SystemManager, bool> isUpdatingChanged = new Signal<SystemManager, bool>();

		private int _frameRate = 60;
		private int _maxUpdates = 5;
		private double _timeTotal = 0; //TO-DO :: Not sure if this should be float, double, or...
		private double _timeElapsedMax = 1;
		private double _timePrevious;

		private SystemManager() : base(false)
		{

		}

		public static SystemManager Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new SystemManager();
				}
				return instance;
			}
		}

		override protected void AddingComponentManager(Entity root)
		{
			base.AddingComponentManager(root);

			root.ComponentAdded.Add(RootComponentAdded, int.MinValue);
			root.ComponentRemoved.Add(RootComponentRemoved, int.MinValue);
			if(root.HasComponent(typeof(EntityManager)))
			{
				RootComponentAdded(root, typeof(EntityManager));
			}

			--TotalSleeping;
		}

		override protected void RemovingComponentManager(Entity root)
		{
			root.ComponentAdded.Remove(RootComponentAdded);
			root.ComponentRemoved.Remove(RootComponentRemoved);
			if(root.HasComponent(typeof(EntityManager)))
			{
				RootComponentRemoved(root, typeof(EntityManager));
			}

			++TotalSleeping;

			base.RemovingComponentManager(root);
		}

		private void RootComponentAdded(Entity root, Type componentType)
		{
			if(componentType == typeof(EntityManager))
			{
				EntityManager entityManager = root.GetComponent(typeof(EntityManager)) as EntityManager;
				entityManager.EntityAdded.Add(EntityAdded, int.MinValue);
				entityManager.EntityRemoved.Add(EntityRemoved, int.MinValue);
				foreach(Entity entity in entityManager.Entities)
				{
					EntityAdded(entityManager, entity);
				}
			}
		}

		private void RootComponentRemoved(Entity root, Type componentType)
		{
			if(componentType == typeof(EntityManager))
			{
				EntityManager entityManager = root.GetComponent(typeof(EntityManager)) as EntityManager;
				entityManager.EntityAdded.Remove(EntityAdded);
				entityManager.EntityRemoved.Remove(EntityRemoved);
				foreach(Entity entity in entityManager.Entities)
				{
					EntityRemoved(entityManager, entity);
				}
			}
		}

		private void EntityAdded(EntityManager entityManager, Entity entity)
		{
			entity.ComponentAdded.Add(ComponentAdded, int.MinValue);
			entity.ComponentRemoved.Add(ComponentRemoved, int.MinValue);
			if(entity.HasComponent(typeof(SystemTypeManager)))
			{
				ComponentAdded(entity, typeof(SystemTypeManager));
			}
		}

		private void EntityRemoved(EntityManager entityManager, Entity entity)
		{
			entity.ComponentAdded.Remove(ComponentAdded);
			entity.ComponentRemoved.Remove(ComponentRemoved);
			if(entity.HasComponent(typeof(SystemTypeManager)))
			{
				ComponentRemoved(entity, typeof(SystemTypeManager));
			}
		}

		private void ComponentAdded(Entity entity, Type componentType)
		{
			if(componentType == typeof(SystemTypeManager))
			{
				SystemTypeManager systemTypeManager = entity.GetComponent(typeof(SystemTypeManager)) as SystemTypeManager;
				systemTypeManager.SystemTypeAdded.Add(SystemTypeAdded, int.MinValue);
				systemTypeManager.SystemTypeRemoved.Add(SystemTypeRemoved, int.MinValue);
				foreach(Type systemType in systemTypeManager.SystemTypes)
				{
					SystemTypeAdded(systemTypeManager, systemType);
				}
			}
		}

		private void ComponentRemoved(Entity entity, Type componentType)
		{
			if(componentType == typeof(SystemTypeManager))
			{
				SystemTypeManager systemTypeManager = entity.GetComponent(typeof(SystemTypeManager)) as SystemTypeManager;
				systemTypeManager.SystemTypeAdded.Remove(SystemTypeAdded);
				systemTypeManager.SystemTypeRemoved.Remove(SystemTypeRemoved);
				foreach(Type systemType in systemTypeManager.SystemTypes)
				{
					SystemTypeRemoved(systemTypeManager, systemType);
				}
			}
		}

		private void SystemTypeAdded(SystemTypeManager systemTypeManager, Type systemType)
		{
			if(!systemTypes.ContainsKey(systemType))
			{
				AtlasSystem system = Activator.CreateInstance(systemType) as AtlasSystem;

				systemTypes.Add(systemType, system);

				system.TotalSleepingChanged.Add(SystemTotalSleepingChanged, int.MinValue);
				if(!system.IsSleeping)
				{
					SystemTotalSleepingChanged(system, 1);
				}

				system.SystemManager = this;

				++system.totalReferences;

				systemAdded.Dispatch(this, systemType);
			}
			else
			{
				++systemTypes[systemType].totalReferences;
			}
		}

		private void SystemTypeRemoved(SystemTypeManager systemTypeManager, Type systemType)
		{
			AtlasSystem system = systemTypes[systemType];

			if(system != null)
			{
				--system.totalReferences;

				if(system.totalReferences == 0)
				{
					systemRemoved.Dispatch(this, systemType);

					system.TotalSleepingChanged.Remove(SystemTotalSleepingChanged);
					if(!system.IsSleeping)
					{
						system.PriorityChanged.Remove(SystemPriorityChanged);
						systems.Remove(system);
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

		private void SystemTotalSleepingChanged(AtlasSystem system, int previousTotalSleeping)
		{
			if(system.TotalSleeping <= 0 && previousTotalSleeping > 0)
			{
				system.PriorityChanged.Add(SystemPriorityChanged, int.MinValue);
				SystemPriorityChanged(system);
			}
			else if(system.TotalSleeping > 0 && previousTotalSleeping <= 0)
			{
				system.PriorityChanged.Remove(SystemPriorityChanged);
				systems.Remove(system);
			}
		}

		private void SystemPriorityChanged(AtlasSystem system, int previousPriority = 0)
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

			systems.Add(system);
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
				return totalSleeping > 0;
			}
		}

		public int TotalSleeping
		{
			get
			{
				return totalSleeping;
			}
			set
			{
				if(totalSleeping != value)
				{
					int previous = totalSleeping;
					totalSleeping = value;

					if(value <= 0 && previous > 0)
					{
						//this._timePrevious = getTimer();
						//this._updater.addEventListener(Event.ENTER_FRAME, this.onEnterFrame);
					}
					else if(value > 0 && previous <= 0)
					{
						//this._updater.removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);
					}

					totalSleepingChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<SystemManager, int> TotalSleepingChanged
		{
			get
			{
				return totalSleepingChanged;
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
