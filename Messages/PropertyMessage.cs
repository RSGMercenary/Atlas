namespace Atlas.Messages
{
	class CollectionMessage<TSender, TKey>:Message<TSender>
	{
		private readonly TKey next;
		private readonly TKey previous;

		public CollectionMessage(string type, TSender first, TSender current, TKey next, TKey previous) : base(type, first, current)
		{
			this.next = next;
			this.previous = previous;
		}

		public TKey Next
		{
			get
			{
				return next;
			}
		}

		public TKey Previous
		{
			get
			{
				return previous;
			}
		}
	}
}
