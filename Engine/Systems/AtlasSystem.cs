using Atlas.Engine.Engine;
using Atlas.Engine.Signals;

namespace Atlas.Engine.Systems
{
	abstract class AtlasSystem:ISystem
	{
		IEngine engine;
		private bool isUpdating = false;
		private bool isUpdatingLocked = false;
		private int priority = 0;
		private int sleeping = 0;
		private bool isDisposing = false;
		private bool isDisposed = false;

		private Signal<ISystem, IEngine, IEngine> engineChanged = new Signal<ISystem, IEngine, IEngine>();
		private Signal<ISystem, bool> isUpdatingChanged = new Signal<ISystem, bool>();
		private Signal<ISystem, int, int> priorityChanged = new Signal<ISystem, int, int>();
		private Signal<ISystem, int, int> sleepingChanged = new Signal<ISystem, int, int>();
		private Signal<ISystem> disposed = new Signal<ISystem>();

		public static implicit operator bool(AtlasSystem system)
		{
			return system != null;
		}

		public AtlasSystem()
		{

		}

		public void Dispose()
		{
			if(!isDisposing)
			{
				Engine = null;
				if(Engine == null)
				{
					isDisposing = true;
					Disposing();
					isDisposing = false;
				}
			}
		}

		protected virtual void Disposing()
		{
			engineChanged.Dispose();
			isUpdatingChanged.Dispose();
			priorityChanged.Dispose();
			sleepingChanged.Dispose();
			disposed.Dispatch(this);
			disposed.Dispose();
		}

		public ISignal<ISystem> Disposed
		{
			get
			{
				return disposed;
			}
		}

		public bool IsDisposing
		{
			get
			{
				return isDisposing;
			}
		}

		public bool IsAutoDisposed
		{
			get
			{
				return true;
			}
			set
			{
				//Systems are always auto disposed.
			}
		}

		public IEngine Engine
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
						IEngine previous = engine;
						engine = value;
						AddingEngine(value);
						engineChanged.Dispatch(this, value, previous);
					}
				}
				else
				{
					if(engine != null && !engine.HasSystem(this))
					{
						IEngine previous = engine;
						engine = value;
						RemovingEngine(previous);
						engineChanged.Dispatch(this, value, previous);
					}
				}
			}
		}

		public ISignal<ISystem, IEngine, IEngine> EngineChanged
		{
			get
			{
				return engineChanged;
			}
		}

		protected virtual void AddingEngine(IEngine engine)
		{

		}

		protected virtual void RemovingEngine(IEngine engine)
		{

		}

		public void Update()
		{
			if(IsSleeping)
				return;
			if(Engine == null)
				return;
			if(Engine.CurrentSystem != this)
				return;
			if(!isUpdatingLocked)
			{
				isUpdatingLocked = true;
				IsUpdating = true;
				Updating();
				IsUpdating = false;
				isUpdatingLocked = false;
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
				if(isUpdating == value)
					return;
				bool previous = isUpdating;
				isUpdating = value;
				isUpdatingChanged.Dispatch(this, value);
			}
		}

		public ISignal<ISystem, bool> IsUpdatingChanged
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
				if(sleeping == value)
					return;
				int previous = sleeping;
				sleeping = value;
				sleepingChanged.Dispatch(this, value, previous);
			}
		}

		public ISignal<ISystem, int, int> SleepingChanged
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
				if(priority == value)
					return;
				int previous = priority;
				priority = value;
				priorityChanged.Dispatch(this, value, previous);
			}
		}

		public ISignal<ISystem, int, int> PriorityChanged
		{
			get
			{
				return priorityChanged;
			}
		}
	}
}