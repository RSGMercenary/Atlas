namespace Atlas.Engine.Messages
{
	class ValueMessage<TSender, TValue> : Message<TSender>, IValueMessage<TSender, TValue>
	{
		private readonly TValue value;

		public ValueMessage(TValue value)
		{
			this.value = value;
		}

		public TValue Value
		{
			get { return value; }
		}
	}
}
