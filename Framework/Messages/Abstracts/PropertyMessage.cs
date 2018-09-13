namespace Atlas.Framework.Messages
{
	public abstract class PropertyMessage<TMessenger, TProperty> : Message<TMessenger>, IPropertyMessage<TMessenger, TProperty>
		where TMessenger : IMessageDispatcher
	{
		public PropertyMessage(TMessenger messenger, TProperty current, TProperty previous) : base(messenger)
		{
			CurrentValue = current;
			PreviousValue = previous;
		}

		public TProperty CurrentValue { get; }
		public TProperty PreviousValue { get; }
	}
}
