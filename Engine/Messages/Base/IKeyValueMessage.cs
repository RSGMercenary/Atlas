namespace Atlas.Engine.Messages
{
	public interface IKeyValueMessage<TMessenger, TKey, TValue> : IMessage<TMessenger>
	{
		TKey Key { get; }
		TValue Value { get; }
	}
}
