namespace Atlas.Engine.Messages
{
	class Message<TSender> : IMessage<TSender>
	{
		private readonly string type = "";
		private TSender target;
		private TSender currentTarget;

		public Message(string type)
		{
			this.type = type;
		}

		public string Type
		{
			get { return type; }
		}

		public TSender Target
		{
			get { return target; }
			set { target = target == null ? value : target; }
		}

		public TSender CurrentTarget
		{
			get { return currentTarget; }
			set { currentTarget = value; }
		}

		public bool AtTarget
		{
			get { return (bool)target?.Equals(currentTarget); }
		}
	}
}