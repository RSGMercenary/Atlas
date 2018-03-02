namespace Atlas.Engine.Messages
{
	class KeyValueMessage<TSender, TKey, TValue> : Message<TSender>, IKeyValueMessage<TSender, TKey, TValue>
	where TSender : IMessageDispatcher
	{
		private readonly TKey key;
		private readonly TValue value;

		public KeyValueMessage(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}

		public TKey Key
		{
			get { return key; }
		}

		public TValue Value
		{
			get { return value; }
		}
	}
}
