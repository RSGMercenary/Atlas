namespace Atlas.Engine.Messages
{
	public class PropertyMessage<TSender, TProperty> : Message<TSender>, IPropertyMessage<TSender, TProperty>
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
