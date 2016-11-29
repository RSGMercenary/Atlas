using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Engine
{
	abstract class EngineObject<T>:IReadOnlyEngineObject<T> where T : EngineObject<T>
	{
		protected IEngine engine;
		private bool isDisposed = false;
		bool isAutoDisposed = true;

		private Signal<T, IEngine, IEngine> engineChanged = new Signal<T, IEngine, IEngine>();
		private Signal<T, bool> isDisposedChanged = new Signal<T, bool>();

		public EngineObject()
		{

		}

		public IEngine Engine
		{
			get
			{
				return engine;
			}
			protected set
			{
				if(engine == value)
					return;
				IEngine previous = engine;
				engine = value;
				//engineChanged.Dispatch(this, value, previous);
			}
		}

		public ISignal<T, IEngine, IEngine> EngineChanged
		{
			get
			{
				return engineChanged;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return isDisposed;
			}
			private set
			{
				if(isDisposed == value)
					return;
				isDisposed = value;
				if(isDisposed)
					Disposing();
				//isDisposedChanged.Dispatch(this, value);
			}
		}

		public bool IsAutoDisposed
		{
			get
			{
				return isAutoDisposed;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected virtual void Disposing()
		{
			engineChanged.Dispose();
			isDisposedChanged.Dispose();
		}
	}
}
