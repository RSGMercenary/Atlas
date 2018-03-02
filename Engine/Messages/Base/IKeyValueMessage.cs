namespace Atlas.Engine.Messages
{
	public interface IKeyValueMessage<TSender, TKey, TValue> : IMessage<TSender>
	{
		TKey Key { get; }
		TValue Value { get; }
	}
}
