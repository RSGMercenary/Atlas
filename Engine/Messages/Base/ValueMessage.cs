namespace Atlas.Engine.Messages
{
	public class ValueMessage<TMessenger, TValue> : Message<TMessenger>, IValueMessage<TMessenger, TValue>
	{
		private readonly TValue value;

		public ValueMessage(TValue value)
		{
			this.value = value;
		}

		public TValue Value
		{
			get { return value; }
		}
	}
}
