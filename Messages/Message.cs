namespace Atlas.Messages
{
	class Message<TSender>:IMessage<TSender>
	{
		private TSender first;
		private TSender current;
		private MessageType type;

		public Message(MessageType type, TSender first, TSender current)
		{
			MessageType = type;
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

		public MessageType MessageType
		{
			get
			{
				return type;
			}
			private set
			{
				type = value;
			}
		}
	}
}
