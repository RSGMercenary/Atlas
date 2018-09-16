namespace Atlas.Core.Messages
{
	public abstract class PropertyMessage<TMessenger, TProperty> : Message<TMessenger>, IPropertyMessage<TMessenger, TProperty>
		where TMessenger : IMessenger
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
