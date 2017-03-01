using Atlas.Engine.Components;
using Atlas.Engine.Signals;

namespace Atlas.Engine
{
	abstract class EngineObject<T>:IEngineObject<T>
		where T : class, IEngineObject<T>
	{
		public static implicit operator bool(EngineObject<T> engineObject)
		{
			return engineObject != null;
		}

		private IEngine engine;
		private EngineObjectState state = EngineObjectState.Destroyed;
		private bool autoDispose = true;

		private Signal<T, IEngine, IEngine> engineChanged = new Signal<T, IEngine, IEngine>();
		private Signal<T, EngineObjectState, EngineObjectState> stateChanged = new Signal<T, EngineObjectState, EngineObjectState>();

		public EngineObject()
		{
			Construct();
		}

		public virtual IEngine Engine
		{
			get
			{
				return engine;
			}
			set
			{
				if(engine == value)
					return;
				var previous = engine;
				engine = value;
				ChangingEngine(value, previous);
				engineChanged.Dispatch(this as T, value, previous);
			}
		}

		protected virtual void ChangingEngine(IEngine current, IEngine previous)
		{

		}

		public ISignal<T, IEngine, IEngine> EngineChanged
		{
			get { return engineChanged; }
		}

		public virtual bool AutoDestroy
		{
			get
			{
				return autoDispose;
			}
			set
			{
				if(autoDispose == value)
					return;
				var previous = autoDispose;
				autoDispose = value;
				ChangingAutoDispose(value, previous);
				//Didn't find a need for this Signal. Could add later.
			}
		}

		protected virtual void ChangingAutoDispose(bool current, bool previous)
		{

		}

		public EngineObjectState State
		{
			get
			{
				return state;
			}
			private set
			{
				if(state == value)
					return;
				var previous = state;
				state = value;
				ChangingState(value, previous);
				stateChanged.Dispatch(this as T, value, previous);
			}
		}

		protected virtual void ChangingState(EngineObjectState current, EngineObjectState previous)
		{

		}

		public ISignal<T, EngineObjectState, EngineObjectState> StateChanged
		{
			get
			{
				return stateChanged;
			}
		}

		protected virtual bool Construct()
		{
			if(state != EngineObjectState.Destroyed)
				return false;
			State = EngineObjectState.Constructing;
			Constructing();
			State = EngineObjectState.Constructed;
			return true;
		}

		public virtual bool Destroy()
		{
			if(state != EngineObjectState.Constructed)
				return false;
			State = EngineObjectState.Destroying;
			Destroying();
			State = EngineObjectState.Destroyed;
			return true;
		}

		protected virtual void Constructing()
		{

		}

		protected virtual void Destroying()
		{

		}
	}
}
