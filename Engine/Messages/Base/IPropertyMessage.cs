namespace Atlas.Engine.Messages
{
	public interface IPropertyMessage<TSender, TProperty> : IMessage<TSender>
	{
		TProperty CurrentValue { get; }
		TProperty PreviousValue { get; }
	}
}
