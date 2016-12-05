namespace Atlas.Testing.Messages
{
	class CollectionMessage<TSender, TKey, TValue>:Message<TSender, TKey, TValue>
	{
		private TKey key;
		private TValue value;
		private bool added = false;

		public CollectionMessage()
		{

		}

		public CollectionMessage(string type, TSender sender, TKey key, TValue value)
		{
			Initialize(type, sender, key, value);
		}

		public void Initialize(string type, TSender sender, TKey key, TValue value)
		{
			Initialize(type, sender);
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
