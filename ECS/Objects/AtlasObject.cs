using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Messages;

namespace Atlas.ECS.Objects
{
	public abstract class AtlasObject<T> : Messenger<T>, IObject<T>
		where T : class, IObject
	{
		private IEngine engine;
		private ObjectState state = ObjectState.Disposed;

		public virtual IEngine Engine
		{
			get { return engine; }
			set
			{
				if(engine == value)
					return;
				var previous = engine;
				engine = value;
				ChangingEngine(value, previous);
				Dispatch<IEngineMessage<T>>(new EngineMessage<T>(this as T, value, previous));
			}
		}

		protected virtual void ChangingEngine(IEngine current, IEngine previous)
		{

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
				Dispatch<IObjectStateMessage<T>>(new ObjectStateMessage<T>(this as T, value, previous));
			}
		}

		public override void Compose()
		{
			if(state != ObjectState.Disposed)
				return;
			State = ObjectState.Composing;
			base.Compose();
			State = ObjectState.Composed;
		}

		public override void Dispose()
		{
			if(state != ObjectState.Composed)
				return;
			State = ObjectState.Disposing;
			base.Dispose();
			State = ObjectState.Disposed;
		}
	}
}
