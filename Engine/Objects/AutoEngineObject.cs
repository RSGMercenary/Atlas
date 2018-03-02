using Atlas.Engine.Messages;

namespace Atlas.Engine
{
	public abstract class AutoEngineObject : EngineObject
	{
		private bool autoDestroy = true;

		public AutoEngineObject()
		{

		}

		public bool AutoDestroy
		{
			get { return autoDestroy; }
			set
			{
				if(autoDestroy == value)
					return;
				var previous = autoDestroy;
				autoDestroy = value;
				Message<IAutoDestroyMessage>(new AutoDestroyMessage(value, previous));
			}
		}
	}
}
