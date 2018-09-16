using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Messages;

namespace Atlas.ECS.Objects
{
	public abstract class EngineObject<T> : AtlasObject<T>, IEngineObject<T>
		where T : class, IEngineObject
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
				Dispatch<IEngineMessage<T>>(new EngineMessage<T>(this as T, value, previous));
			}
		}
	}
}
