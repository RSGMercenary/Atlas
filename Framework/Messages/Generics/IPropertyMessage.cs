namespace Atlas.Framework.Messages
{
	public interface IPropertyMessage<TMessenger, TProperty> : IMessage<TMessenger>
		where TMessenger : IMessageDispatcher
	{
		TProperty CurrentValue { get; }
		TProperty PreviousValue { get; }
	}
}
