namespace Atlas.Engine.Messages
{
	interface IValueMessage<TSender, TValue> : IMessage<TSender>
	{
		TValue Value { get; }
	}
}
