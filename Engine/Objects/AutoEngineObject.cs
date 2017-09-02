using Atlas.Engine.Messages;

namespace Atlas.Engine
{
	public abstract class AutoEngineObject<T> : EngineObject<T>
		where T : class, IAutoEngineObject<T>
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
				Message(new PropertyMessage<T, bool>(AtlasMessage.AutoDestroy, value, previous));
			}
		}
	}
}
