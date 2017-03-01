namespace Atlas.Engine.Collections.LinkList
{
	class LinkListNode<T>:ILinkListNode<T>
	{
		public static implicit operator bool(LinkListNode<T> node)
		{
			return node != null;
		}

		public LinkList<T> linklist;
		public LinkListNode<T> previous;
		public LinkListNode<T> next;
		public T value;

		public ILinkList<T> LinkList
		{
			get
			{
				return linklist;
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
