namespace Atlas.Core.Messages
{
	public interface IKeyValueMessage<TMessenger, out TKey, out TValue> : IValueMessage<TMessenger, TValue>
		where TMessenger : IMessenger
	{
		TKey Key { get; }
	}
}