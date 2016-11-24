using Atlas.Engine.Engine;
using Atlas.Engine.Signals;

namespace Atlas.Engine.Systems
{
	abstract class AtlasSystem:ISystem
	{
		private IEngineManager engine;
		private Signal<ISystem, IEngineManager, IEngineManager> engineChanged = new Signal<ISystem, IEngineManager, IEngineManager>();

		private int sleeping = 0;
		private Signal<ISystem, int, int> sleepingChanged = new Signal<ISystem, int, int>();

		private int priority = 0;
		private Signal<ISystem, int, int> priorityChanged = new Signal<ISystem, int, int>();

		private bool isUpdating = false;
		private Signal<ISystem, bool, bool> isUpdatingChanged = new Signal<ISystem, bool, bool>();

		private bool isDisposed = false;
		private Signal<ISystem, bool, bool> isDisposedChanged = new Signal<ISystem, bool, bool>();

		public static implicit operator bool(AtlasSystem system)
		{
			return system != null;
		}

		public AtlasSystem()
		{

		}

		public void Dispose()
		{
			if(engine == null)
			{
				IsDisposed = true;
				Disposing();
				engineChanged.Dispose();
				isUpdatingChanged.Dispose();
				priorityChanged.Dispose();
				sleepingChanged.Dispose();
			}
		}

		protected virtual void Disposing()
		{

		}

		public bool IsDisposed
		{
			get
			{
				return IsDisposed;
			}
			private set
			{
				if(isDisposed != value)
				{
					bool previous = isDisposed;
					isDisposed = value;
					isDisposedChanged.Dispatch(this, value, previous);
				}
			}
		}

		public Signal<ISystem, bool, bool> IsDisposedChanged
		{
			get
			{
				return isDisposedChanged;
			}
		}

		public bool IsDisposedWhenUnmanaged
		{
			get
			{
				return true;
			}
			set
			{

			}
		}

		public IEngineManager Engine
		{
			get
			{
				return engine;
			}
			set
			{
				if(value != null)
				{
					if(engine == null && value.HasSystem(this))
					{
						IEngineManager previous = engine;
						engine = value;
						AddingEngine(value);
						engineChanged.Dispatch(this, value, previous);
					}
				}
				else
				{
					if(engine != null && !engine.HasSystem(this))
					{
						IEngineManager previous = engine;
						engine = value;
						RemovingEngine(previous);
						engineChanged.Dispatch(this, value, previous);
					}
				}
			}
		}

		protected virtual void AddingEngine(IEngineManager engine)
		{

		}

		protected virtual void RemovingEngine(IEngineManager engine)
		{

		}

		public ISignal<ISystem, IEngineManager, IEngineManager> EngineChanged
		{
			get
			{
				return engineChanged;
			}
		}

		public void Update()
		{
			if(IsSleeping)
				return;
			if(engine == null)
				return;
			if(engine.CurrentSystem != this)
				return;
			if(!IsUpdating)
			{
				IsUpdating = true;
				Updating();
				IsUpdating = false;
			}
		}

		protected virtual void Updating()
		{

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

		public Signal<ISystem, bool, bool> IsUpdatingChanged
		{
			get
			{
				return isUpdatingChanged;
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

		public Signal<ISystem, int, int> SleepingChanged
		{
			get
			{
				return sleepingChanged;
			}
		}

		public bool IsSleeping
		{
			get
			{
				return sleeping > 0;
			}
		}

		public int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				if(priority != value)
				{
					int previous = priority;
					priority = value;
					priorityChanged.Dispatch(this, value, previous);
				}
			}
		}

		public Signal<ISystem, int, int> PriorityChanged
		{
			get
			{
				return priorityChanged;
			}
		}
	}
}