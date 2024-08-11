namespace Atlas.Core.Messages;

public abstract class ValueMessage<TMessenger, TValue> : Message<TMessenger>, IValueMessage<TMessenger, TValue>
	where TMessenger : IMessenger
{
	protected ValueMessage(TValue value)
	{
		Value = value;
	}

	public TValue Value { get; }
}