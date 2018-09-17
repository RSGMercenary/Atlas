namespace Atlas.Core.Messages
{
	public interface IValueMessage<out TMessenger, TValue> : IMessage<TMessenger>
		where TMessenger : IMessenger
	{
		TValue Value { get; }
	}
}
