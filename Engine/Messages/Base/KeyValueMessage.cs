namespace Atlas.Engine.Messages
{
	public class KeyValueMessage<TMessenger, TKey, TValue> : Message<TMessenger>, IKeyValueMessage<TMessenger, TKey, TValue>
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
