namespace Atlas.Core.Messages
{
	public interface IPropertyMessage<out TMessenger, TProperty> : IMessage<TMessenger>
		where TMessenger : IMessenger
	{
		TProperty CurrentValue { get; }
		TProperty PreviousValue { get; }
	}
}
