namespace Atlas.Engine.Messages
{
	class KeyValueMessage<TSender, TKey, TValue> : Message<TSender>, IKeyValueMessage<TSender, TKey, TValue>
	{
		private readonly TKey key;
		private readonly TValue value;

		public KeyValueMessage(string type, TKey key, TValue value) : base(type)
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
