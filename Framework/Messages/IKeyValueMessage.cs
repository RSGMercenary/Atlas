namespace Atlas.Framework.Messages
{
	public interface IKeyValueMessage<TMessenger, TKey, TValue> : IValueMessage<TMessenger, TValue>
		where TMessenger : IMessageDispatcher
	{
		TKey Key { get; }
	}
}
