using Atlas.Components;
using Atlas.Entities;
using Atlas.LinkList;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Systems
{
	sealed class SystemManager:Component, ISystemManager
	{
		private static SystemManager instance;

		private List<ISystem> systems = new List<ISystem>();
		private Dictionary<Type, ISystem> systemTypes = new Dictionary<Type, ISystem>();
		private Signal<SystemManager, Type> systemAdded = new Signal<SystemManager, Type>();
		private Signal<SystemManager, Type> systemRemoved = new Signal<SystemManager, Type>();
		private List<SystemX> systemsRemoved = new List<SystemX>();

		private int sleeping = 1;
		private Signal<SystemManager, int> sleepingChanged = new Signal<SystemManager, int>();

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
					instance = new SystemManager();
				return instance;
			}
		}

		override protected void AddingComponentManager(IEntity root)
		{
			base.AddingComponentManager(root);

			root.ComponentAdded.Add(RootComponentAdded, int.MinValue);
			root.ComponentRemoved.Add(RootComponentRemoved, int.MinValue);
			if(root.HasComponent<IEntityManager>())
			{
				RootComponentAdded(root, root.GetComponent<IEntityManager>(), typeof(IEntityManager));
			}

			--Sleeping;
		}

		override protected void RemovingComponentManager(IEntity root)
		{
			root.ComponentAdded.Remove(RootComponentAdded);
			root.ComponentRemoved.Remove(RootComponentRemoved);
			if(root.HasComponent<IEntityManager>())
			{
				RootComponentRemoved(root, root.GetComponent<IEntityManager>(), typeof(IEntityManager));
			}

			++Sleeping;

			base.RemovingComponentManager(root);
		}

		private void RootComponentAdded(IEntity root, IComponent component, Type componentType)
		{
			if(componentType == typeof(IEntityManager))
			{
				IEntityManager entityManager = root.GetComponent<IEntityManager>();
				entityManager.EntityAdded.Add(EntityManagerEntityAdded, int.MinValue);
				entityManager.EntityRemoved.Add(EntityManagerEntityRemoved, int.MinValue);
				ILinkListNode<IEntity> current = entityManager.Entities.First;
				while(current != null)
				{
					EntityManagerEntityAdded(entityManager, current.Value);
					current = current.Next;
				}
			}
		}

		private void RootComponentRemoved(IEntity root, IComponent component, Type componentType)
		{
			if(componentType == typeof(IEntityManager))
			{
				IEntityManager entityManager = root.GetComponent<IEntityManager>();
				entityManager.EntityAdded.Remove(EntityManagerEntityAdded);
				entityManager.EntityRemoved.Remove(EntityManagerEntityRemoved);
				ILinkListNode<IEntity> current = entityManager.Entities.First;
				while(current != null)
				{
					EntityManagerEntityRemoved(entityManager, current.Value);
					current = current.Next;
				}
			}
		}

		private void EntityManagerEntityAdded(IEntityManager entityManager, IEntity entity)
		{
			entity.SystemTypeAdded.Add(EntitySystemTypeAdded, int.MinValue);
			entity.SystemTypeRemoved.Add(EntitySystemTypeRemoved, int.MinValue);
		}

		private void EntityManagerEntityRemoved(IEntityManager entityManager, IEntity entity)
		{
			entity.SystemTypeAdded.Remove(EntitySystemTypeAdded);
			entity.SystemTypeRemoved.Remove(EntitySystemTypeRemoved);
		}

		private void EntitySystemTypeAdded(IEntity entity, Type type)
		{
			if(!systemTypes.ContainsKey(type))
			{
				ISystem system = Activator.CreateInstance(type) as ISystem;

				systemTypes.Add(type, system);

				system.PriorityChanged.Add(SystemPriorityChanged);
				SystemPriorityChanged(system, 0, 0);

				system.SystemManager = this;

				system.AddSystemManager(entity);

				systemAdded.Dispatch(this, type);
			}
			else
			{
				systemTypes[type].AddSystemManager(entity);
			}
		}

		private void EntitySystemTypeRemoved(IEntity entity, Type type)
		{
			ISystem system = systemTypes[type];

			if(system != null)
			{
				system.RemoveSystemManager(entity);

				if(system.NumSystemManagers == 0)
				{
					systemRemoved.Dispatch(this, type);

					system.PriorityChanged.Remove(SystemPriorityChanged);

					systemTypes.Remove(type);

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

		private void SystemPriorityChanged(ISystem system, int current, int previous)
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

		public bool HasSystem(ISystem system)
		{
			return systemTypes.ContainsKey(system.GetType()) && systemTypes[system.GetType()] == system;
		}

		public bool HasSystem<T>()
		{
			return HasSystem(typeof(T));
		}

		public bool HasSystem(Type systemType)
		{
			return systemTypes.ContainsKey(systemType);
		}

		public SystemX GetSystem(Type type)
		{
			return systemTypes.ContainsKey(type) ? systemTypes[type] : null;
		}

		public SystemX GetSystem(int index)
		{
			if(index < 0)
				return null;
			if(index > systems.Count - 1)
				return null;
			return systems[index];
		}

		public int GetSystemIndex(SystemX system)
		{
			return systems.IndexOf(system);
		}

		public List<SystemX> Systems
		{
			get
			{
				return new List<SystemX>(systems);
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

		public void Update()
		{
			if(!IsUpdating)
			{
				IsUpdating = true;

				if(systems.Count > 0)
				{
					List<SystemX> updateSystems = new List<SystemX>(systems);
					foreach(SystemX system in updateSystems)
					{
						if(!system.IsSleeping)
						{
							system.Update();
						}
					}
				}

				while(systemsRemoved.Count > 0)
				{
					ISystem system = systemsRemoved[systemsRemoved.Count - 1];
					systemsRemoved.RemoveAt(systemsRemoved.Count - 1);
					system.Dispose();
				}

				IsUpdating = false;
			}
		}
	}
}
