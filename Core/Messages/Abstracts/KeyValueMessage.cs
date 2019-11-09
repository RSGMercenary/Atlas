namespace Atlas.Core.Messages
{
	public abstract class KeyValueMessage<TMessenger, TKey, TValue> : ValueMessage<TMessenger, TValue>, IKeyValueMessage<TMessenger, TKey, TValue>
		where TMessenger : IMessenger
	{
		protected KeyValueMessage(TKey key, TValue value) : base(value)
		{
			Key = key;
		}

		public TKey Key { get; }
	}
}