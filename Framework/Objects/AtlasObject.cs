using Atlas.Framework.Messages;
using System;

namespace Atlas.Framework.Objects
{
	public abstract class AtlasObject : MessageDispatcher, IObject
	{
		public static implicit operator bool(AtlasObject engineObject)
		{
			return engineObject != null;
		}

		private ObjectState state = ObjectState.Disposed;

		public AtlasObject()
		{
			Initialize(true);
		}

		~AtlasObject()
		{
			Dispose(true);
		}

		public ObjectState State
		{
			get { return state; }
			private set
			{
				if(state == value)
					return;
				var previous = state;
				state = value;
				Message<IObjectStateMessage>(new ObjectStateMessage(value, previous));
			}
		}

		internal void Initialize()
		{
			Initialize(false);
		}

		private void Initialize(bool constructor)
		{
			if(state != ObjectState.Disposed)
				return;
			State = ObjectState.Initializing;
			Initializing(constructor);
			State = ObjectState.Initialized;
			GC.ReRegisterForFinalize(this);
		}

		public virtual void Dispose()
		{
			Dispose(false);
		}

		private void Dispose(bool finalizer)
		{
			if(state != ObjectState.Initialized)
				return;
			State = ObjectState.Disposing;
			Disposing(finalizer);
			State = ObjectState.Disposed;
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Called when this instance is being constructed. Should not be called manually.
		/// </summary>
		virtual protected void Initializing(bool constructor)
		{

		}

		/// <summary>
		/// Called when this instance is being destroyed. Should not be called manually.
		/// </summary>
		virtual protected void Disposing(bool finalizer)
		{

		}
	}
}
