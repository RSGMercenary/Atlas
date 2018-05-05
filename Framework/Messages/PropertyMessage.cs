namespace Atlas.Framework.Messages
{
	public class PropertyMessage<TMessenger, TProperty> : Message<TMessenger>, IPropertyMessage<TMessenger, TProperty>
		where TMessenger : IMessageDispatcher
	{
		public PropertyMessage()
		{

		}

		public PropertyMessage(TProperty current, TProperty previous)
		{
			CurrentValue = current;
			PreviousValue = previous;
		}

		public TProperty CurrentValue { get; set; }
		public TProperty PreviousValue { get; set; }
	}
}
