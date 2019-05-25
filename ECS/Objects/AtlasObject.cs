using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Messages;

namespace Atlas.ECS.Objects
{
	public abstract class AtlasObject<T> : Messenger<T>, IObject<T>
		where T : class, IObject
	{
		private IEngine engine;

		public virtual IEngine Engine
		{
			get { return engine; }
			set
			{
				if(engine == value)
					return;
				var previous = engine;
				engine = value;
				if(value != null)
					AddingEngine(value);
				else
					RemovingEngine(previous);
				Message<IEngineMessage<T>>(new EngineMessage<T>(this as T, value, previous));
			}
		}

		protected virtual void AddingEngine(IEngine engine) { }

		protected virtual void RemovingEngine(IEngine engine) { }
	}
}