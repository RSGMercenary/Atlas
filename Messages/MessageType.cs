namespace Atlas.Messages
{
	class MessageType
	{
		private string description = "";

		protected MessageType(string description)
		{
			Description = description;
		}

		public string Description
		{
			get
			{
				return description;
			}
			private set
			{
				if(string.IsNullOrWhiteSpace(value))
					return;
				description = value;
			}
		}
	}
}
