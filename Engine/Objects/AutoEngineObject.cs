namespace Atlas.Engine
{
	class AutoEngineObject<T> : EngineObject<T>
		where T : class, IAutoEngineObject<T>
	{
		private bool autoDestroy = true;

		public AutoEngineObject()
		{

		}

		public bool AutoDestroy
		{
			get
			{
				return autoDestroy;
			}
			set
			{
				if(autoDestroy == value)
					return;
				var previous = autoDestroy;
				autoDestroy = value;
				ChangingAutoDestroy(value, previous);
				//Didn't find a need for this Signal. Could add later.
			}
		}

		protected virtual void ChangingAutoDestroy(bool current, bool previous)
		{

		}
	}
}
