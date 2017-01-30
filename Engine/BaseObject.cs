using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;

namespace Atlas.Engine
{
	abstract class BaseObject<T>:IDispose<T>, IAutoDispose
		where T : class, IBaseObject<T>
	{
		private bool autoDispose = true;
		private bool isDisposing = false;
		private bool isDisposed = false;

		private Signal<T> disposed = new Signal<T>();

		public virtual bool AutoDispose
		{
			get
			{
				return autoDispose;
			}
			set
			{
				if(autoDispose == value)
					return;
				autoDispose = value;
				ChangingAutoDispose();
			}
		}

		protected virtual void ChangingAutoDispose()
		{

		}

		public bool IsDisposing
		{
			get
			{
				return isDisposing;
			}
			private set
			{
				isDisposing = value;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return isDisposed;
			}
			protected set
			{
				isDisposed = value;
			}
		}

		public ISignal<T> Disposed
		{
			get
			{
				return disposed;
			}
		}

		public virtual void Dispose()
		{
			if(isDisposed || isDisposing)
				return;
			IsDisposing = true;
			Disposing();
			IsDisposing = false;
			IsDisposed = true;
		}

		protected virtual void Disposing()
		{
			disposed.Dispatch(this as T);
			disposed.Dispose();
		}
	}
}
