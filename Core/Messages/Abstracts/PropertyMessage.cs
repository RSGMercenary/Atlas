namespace Atlas.Core.Messages
{
	public abstract class PropertyMessage<TMessenger, TProperty> : Message<TMessenger>, IPropertyMessage<TMessenger, TProperty>
		where TMessenger : class, IMessenger
	{
		public PropertyMessage(TProperty current, TProperty previous)
		{
			CurrentValue = current;
			PreviousValue = previous;
		}

		public TProperty CurrentValue { get; }
		public TProperty PreviousValue { get; }
	}
}