namespace Atlas.Core.Messages
{
	public interface IKeyValueMessage<TMessenger, TKey, TValue> : IValueMessage<TMessenger, TValue>
		where TMessenger : IMessenger
	{
		TKey Key { get; }
	}
}
