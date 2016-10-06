using System.Collections.Generic;

namespace Atlas.NewNodes
{
	class WriteLinkedList<T>:ReadLinkedList<T>
	{
		public WriteLinkedList()
		{
			LinkedList<int> g;
		}

		public AtlasNode<T> Add(T data, int index)
		{
			if(data != null)
			{
				for(AtlasNode<T> node = first; node; node = node.next)
				{
					if(ReferenceEquals(node.data, data))
					{
						return null;
					}
				}
			}
			return null;
		}

		public AtlasNode<T> Remove(T data)
		{
			if(data != null)
			{
				for(AtlasNode<T> node = first; node; node = node.next)
				{
					if(ReferenceEquals(node.data, data))
					{

					}
				}
			}
			return null;
		}
	}
}
