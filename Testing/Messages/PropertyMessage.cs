namespace Atlas.Testing.Messages
{
	class PropertyMessage<TSender, TProperty>:Message<TSender, TProperty, TProperty>
	{
		private TProperty next;
		private TProperty previous;

		public PropertyMessage()
		{

		}

		public PropertyMessage(string type, TSender sender, TProperty next, TProperty previous)
		{
			Initialize(type, sender, next, previous);
		}

		public void Initialize(string type, TSender sender, TProperty next, TProperty previous)
		{
			Initialize(type, sender);
			this.next = next;
			this.previous = previous;
		}

		public TProperty Next
		{
			get
			{
				return next;
			}
		}

		public TProperty Previous
		{
			get
			{
				return previous;
			}
		}
	}
}
