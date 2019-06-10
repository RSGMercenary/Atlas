namespace Atlas.Core.Messages
{
	public abstract class ValueMessage<TMessenger, TValue> : Message<TMessenger>, IValueMessage<TMessenger, TValue>
		where TMessenger : class, IMessenger
	{
		public ValueMessage(TValue value)
		{
			Value = value;
		}

		public TValue Value { get; }
	}
}