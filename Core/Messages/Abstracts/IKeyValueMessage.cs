namespace Atlas.Core.Messages
{
	public interface IKeyValueMessage<out TMessenger, TKey, TValue> : IValueMessage<TMessenger, TValue>
		where TMessenger : IMessenger
	{
		TKey Key { get; }
	}
}
