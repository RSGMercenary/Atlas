namespace Atlas.Core.Messages;

public interface IPropertyMessage<out TMessenger, out TProperty> : IMessage<TMessenger>
		where TMessenger : IMessenger
{
	TProperty CurrentValue { get; }
	TProperty PreviousValue { get; }
}