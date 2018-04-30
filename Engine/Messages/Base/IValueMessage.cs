namespace Atlas.Engine.Messages
{
	public interface IValueMessage<TMessenger, TValue> : IMessage<TMessenger>
	{
		TValue Value { get; }
	}
}
