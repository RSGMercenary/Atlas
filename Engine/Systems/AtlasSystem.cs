using Atlas.Engine.Components;
using Atlas.Engine.Signals;

namespace Atlas.Engine.Systems
{
	abstract class AtlasSystem:EngineObject<ISystem>, ISystem
	{
		private bool isUpdating = false;
		private bool isUpdatingLocked = false;
		private int priority = 0;
		private int sleeping = 0;

		private Signal<ISystem, bool> isUpdatingChanged = new Signal<ISystem, bool>();
		private Signal<ISystem, int, int> priorityChanged = new Signal<ISystem, int, int>();
		private Signal<ISystem, int, int> sleepingChanged = new Signal<ISystem, int, int>();

		public AtlasSystem()
		{

		}

		sealed public override bool Destroy()
		{
			if(State != EngineObjectState.Constructed)
				return false;
			Engine = null;
			if(Engine == null)
				return base.Destroy();
			return false;
		}

		protected override void Destroying()
		{
			isUpdatingChanged.Dispose();
			priorityChanged.Dispose();
			sleepingChanged.Dispose();
			base.Destroying();
		}

		sealed public override bool AutoDestroy
		{
			get
			{
				return base.AutoDestroy;
			}
			set
			{
				//Systems are always auto disposed.
			}
		}

		sealed override public IEngine Engine
		{
			get
			{
				return base.Engine;
			}
			set
			{
				if(value != null)
				{
					if(Engine == null && value.HasSystem(this))
					{
						base.Engine = value;
					}
				}
				else
				{
					if(Engine != null && !Engine.HasSystem(this))
					{
						base.Engine = value;
					}
				}
			}
		}

		protected override void ChangingEngine(IEngine current, IEngine previous)
		{
			base.ChangingEngine(current, previous);
			if(current != null)
			{
				AddingEngine(current);
			}
			else if(previous != null)
			{
				RemovingEngine(previous);
			}
		}

		protected virtual void AddingEngine(IEngine engine)
		{

		}

		protected virtual void RemovingEngine(IEngine engine)
		{

		}

		public void FixedUpdate(double deltaTime)
		{
			if(IsSleeping)
				return;
			if(Engine == null)
				return;
			if(Engine.CurrentFixedUpdateSystem != this)
				return;
			if(!isUpdatingLocked)
			{
				isUpdatingLocked = true;
				IsUpdating = true;
				FixedUpdating(deltaTime);
				IsUpdating = false;
				isUpdatingLocked = false;
			}
		}

		protected virtual void FixedUpdating(double deltaTime)
		{

		}

		public void Update(double deltaTime)
		{
			if(IsSleeping)
				return;
			if(Engine == null)
				return;
			if(Engine.CurrentUpdateSystem != this)
				return;
			if(!isUpdatingLocked)
			{
				isUpdatingLocked = true;
				IsUpdating = true;
				Updating(deltaTime);
				IsUpdating = false;
				isUpdatingLocked = false;
			}
		}

		protected virtual void Updating(double deltaTime)
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