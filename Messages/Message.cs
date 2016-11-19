namespace Atlas.Messages
{
	class Message<TSender>:IMessage<TSender>
	{
		private TSender first;
		private TSender current;
		private string type = "";

		public Message(string type, TSender first, TSender current)
		{
			Type = type;
			First = first;
			Current = current;
		}

		public TSender First
		{
			get
			{
				return first;
			}
			private set
			{
				first = value;
			}
		}

		public TSender Current
		{
			get
			{
				return current;
			}
			private set
			{
				current = value;
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
				if(string.IsNullOrWhiteSpace(value))
					return;
				type = value;
			}
		}
	}
}
