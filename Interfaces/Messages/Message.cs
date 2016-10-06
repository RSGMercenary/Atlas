namespace Atlas.Interfaces.Messages
{
	class Message<Sender>:IMessage<Sender> where Sender : class
	{
		private string type = "";
		private Sender first;
		private Sender current;

		public Message(string type)
		{
			Type = type;
		}

		public Sender Current
		{
			get
			{
				return current;
			}
			set
			{
				if(current != value)
				{
					current = value;
				}
			}
		}

		public Sender First
		{
			get
			{
				return first;
			}
			set
			{
				if(first != value)
				{
					first = value;
				}
			}
		}

		public string Type
		{
			get
			{
				return type;
			}
			private set
			{
				if(!string.IsNullOrWhiteSpace(value))
				{
					if(type != value)
					{
						type = value;
					}
				}
			}
		}
	}
}
