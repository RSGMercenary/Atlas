namespace Atlas.Engine.Messages
{
	class ValueMessage<TSender, TValue> : Message<TSender>, IValueMessage<TSender, TValue>
	{
		private readonly TValue value;

		public ValueMessage(string type, TValue value) : base(type)
		{
			this.value = value;
		}

		public TValue Value
		{
			get { return value; }
		}
	}
}
