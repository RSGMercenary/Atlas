namespace Atlas.Framework.Messages
{
	public class KeyValueMessage<TMessenger, TKey, TValue> : ValueMessage<TMessenger, TValue>, IKeyValueMessage<TMessenger, TKey, TValue>
		where TMessenger : IMessageDispatcher
	{
		public KeyValueMessage()
		{

		}

		public KeyValueMessage(TKey key, TValue value) : base(value)
		{
			Key = key;
		}

		public TKey Key { get; set; }
	}
}
