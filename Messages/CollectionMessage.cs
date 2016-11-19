namespace Atlas.Messages
{
	class CollectionMessage<TSender, TKey, TValue>:Message<TSender>
	{
		private readonly TKey key;
		private readonly TValue value;

		public CollectionMessage(string type, TSender first, TSender current, TKey key, TValue value) : base(type, first, current)
		{
			this.key = key;
			this.value = value;
		}

		public TKey Key
		{
			get
			{
				return key;
			}
		}

		public TValue Value
		{
			get
			{
				return value;
			}
		}
	}
}
