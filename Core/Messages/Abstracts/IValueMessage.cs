namespace Atlas.Core.Messages
{
	public interface IValueMessage<TMessenger, TValue> : IMessage<TMessenger>
		where TMessenger : IMessenger
	{
		TValue Value { get; }
	}
}
