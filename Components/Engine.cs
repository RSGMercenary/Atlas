namespace Atlas.Components
{
	class Engine:Component
	{
		private static Engine instance;

		private Engine() : base(false)
		{

		}

		public static Engine Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new Engine();
				}
				return instance;
			}
		}
	}
}
