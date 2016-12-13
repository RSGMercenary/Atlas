namespace Atlas.Testing.Messages
{
	class CollectionMessage<TDispatcher, TKey, TValue>:Message<TDispatcher, TKey, TValue>
	{
		private TKey key;
		private TValue value;
		private bool added = false;

		public CollectionMessage()
		{

		}

		public CollectionMessage(string type, TDispatcher source, TDispatcher sender, TKey key, TValue value)
		{
			Initialize(type, source, sender, key, value);
		}

		public void Initialize(string type, TDispatcher source, TDispatcher sender, TKey key, TValue value)
		{
			Initialize(type, source, sender);
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
