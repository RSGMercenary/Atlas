namespace Atlas.LinkList
{
	class LinkListNode<T>:ILinkListNode<T>
	{
		public LinkList<T> nodelist;
		public LinkListNode<T> previous;
		public LinkListNode<T> next;
		public T value;

		public ILinkList<T> NodeList
		{
			get
			{
				return nodelist;
			}
		}

		public ILinkListNode<T> Previous
		{
			get
			{
				return previous;
			}
		}

		public ILinkListNode<T> Next
		{
			get
			{
				return next;
			}
		}

		public T Value
		{
			get
			{
				return value;
			}
		}
	}
}
