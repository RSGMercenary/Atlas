namespace Atlas.NewNodes
{
	class AtlasNode<T>
	{
		internal AtlasNode<T> previous;
		internal AtlasNode<T> next;
		internal T data;

		public static implicit operator bool(AtlasNode<T> node)
		{
			return node != null;
		}

		internal AtlasNode()
		{

		}

		public AtlasNode<T> Previous
		{
			get
			{
				return previous;
			}
		}

		public AtlasNode<T> Next
		{
			get
			{
				return next;
			}
		}

		public T Data
		{
			get
			{
				return data;
			}
		}
	}
}
