namespace Atlas.Engine.Messages
{
	interface IValueMessage<TMessenger, TValue> : IMessage<TMessenger>
	{
		TValue Value { get; }
	}
}
