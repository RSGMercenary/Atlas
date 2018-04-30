namespace Atlas.Engine.Messages
{
	public interface IPropertyMessage<TMessenger, TProperty> : IMessage<TMessenger>
	{
		TProperty CurrentValue { get; }
		TProperty PreviousValue { get; }
	}
}
