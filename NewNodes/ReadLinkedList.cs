namespace Atlas.NewNodes
{
	class ReadLinkedList<T>
	{
		protected AtlasNode<T> first;
		protected AtlasNode<T> last;

		public ReadLinkedList()
		{

		}

		public AtlasNode<T> First
		{
			get
			{
				return first;
			}
		}

		public AtlasNode<T> Last
		{
			get
			{
				return last;
			}
		}
	}
}
