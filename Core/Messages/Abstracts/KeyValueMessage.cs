namespace Atlas.Core.Messages
{
	public abstract class KeyValueMessage<TMessenger, TKey, TValue> : ValueMessage<TMessenger, TValue>, IKeyValueMessage<TMessenger, TKey, TValue>
		where TMessenger : IMessageDispatcher
	{
		public KeyValueMessage(TMessenger messenger, TKey key, TValue value) : base(messenger, value)
		{
			Key = key;
		}

		public TKey Key { get; }
	}
}
