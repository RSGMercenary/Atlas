namespace Atlas.Engine.Messages
{
	public class PropertyMessage<TMessenger, TProperty> : Message<TMessenger>, IPropertyMessage<TMessenger, TProperty>
	{
		private readonly TProperty current;
		private readonly TProperty previous;

		public PropertyMessage(TProperty current, TProperty previous)
		{
			this.current = current;
			this.previous = previous;
		}

		public TProperty CurrentValue
		{
			get { return current; }
		}

		public TProperty PreviousValue
		{
			get { return previous; }
		}
	}
}
