namespace Atlas.Engine.Messages
{
	interface IPropertyMessage<TSender, TProperty> : IMessage<TSender>
	{
		TProperty Current { get; }
		TProperty Previous { get; }
	}
}
