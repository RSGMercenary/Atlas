namespace Atlas.Core.Messages
{
	public interface IValueMessage<out TMessenger, out TValue> : IMessage<TMessenger>
		where TMessenger : IMessenger
	{
		TValue Value { get; }
	}
}