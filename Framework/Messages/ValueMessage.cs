namespace Atlas.Framework.Messages
{
	public class ValueMessage<TMessenger, TValue> : Message<TMessenger>, IValueMessage<TMessenger, TValue>
		where TMessenger : IMessageDispatcher
	{
		public ValueMessage()
		{

		}

		public ValueMessage(TValue value)
		{
			Value = value;
		}

		public TValue Value { get; set; }
	}
}
