namespace Atlas.Framework.Messages
{
	public abstract class ValueMessage<TMessenger, TValue> : Message<TMessenger>, IValueMessage<TMessenger, TValue>
		where TMessenger : IMessageDispatcher
	{
		public ValueMessage(TMessenger messenger, TValue value) : base(messenger)
		{
			Value = value;
		}

		public TValue Value { get; }
	}
}
