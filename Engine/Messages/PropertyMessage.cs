namespace Atlas.Engine.Messages
{
	class PropertyMessage<TSender, TProperty> : Message<TSender>, IPropertyMessage<TSender, TProperty>
	{
		private readonly TProperty current;
		private readonly TProperty previous;

		public PropertyMessage(string type, TProperty current, TProperty previous) : base(type)
		{
			this.current = current;
			this.previous = previous;
		}

		public TProperty Current
		{
			get { return current; }
		}

		public TProperty Previous
		{
			get { return previous; }
		}
	}
}
