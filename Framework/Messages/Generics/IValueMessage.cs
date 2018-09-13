namespace Atlas.Framework.Messages
{
	public interface IValueMessage<TMessenger, TValue> : IMessage<TMessenger>
		where TMessenger : IMessageDispatcher
	{
		TValue Value { get; }
	}
}
