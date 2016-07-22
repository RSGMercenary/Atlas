using Atlas.Entities;

namespace Atlas.Nodes
{
	abstract class Node
	{
		//private NodeList nodeList;
		private Node previous;
		private Node next;
		private Entity entity;

		internal Node()
		{

		}

		/*public NodeList NodeList
		{
			get
			{
				return nodeList;
			}
			internal set
			{
				nodeList = value;
			}
		}*/

		public Node Previous
		{
			get
			{
				return previous;
			}
			internal set
			{
				previous = value;
			}
		}

		public Node Next
		{
			get
			{
				return next;
			}
			internal set
			{
				next = value;
			}
		}

		public Entity Entity
		{
			get
			{
				return entity;
			}
			internal set
			{
				entity = value;
			}
		}
	}
}
