using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Messages;

namespace Atlas.ECS.Objects
{
	public abstract class EngineObject : AtlasObject, IEngineObject
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
				Message<IEngineMessage>(new EngineMessage(this, value, previous));
			}
		}
	}
}
