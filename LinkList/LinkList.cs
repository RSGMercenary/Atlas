using System.Collections.Generic;

namespace Atlas.LinkList
{
	class LinkList<T>:ILinkList<T>
	{
		private Stack<LinkListNode<T>> pool = new Stack<LinkListNode<T>>();
		private LinkListNode<T> first;
		private LinkListNode<T> last;
		private int count = 0;

		public T this[int i]
		{
			get
			{
				return Get(i);
			}
			//set { InnerList[i] = value; }
		}

		public ILinkListNode<T> First
		{
			get
			{
				return first;
			}
		}

		public ILinkListNode<T> Last
		{
			get
			{
				return last;
			}
		}

		public int Count
		{
			get
			{
				return count;
			}
		}

		protected LinkListNode<T> CreateNode()
		{
			return new LinkListNode<T>();
		}

		public bool Contains(T data)
		{
			if(data == null)
				return false;
			for(LinkListNode<T> current = first; current != null; current = current.next)
			{
				if(current.value.Equals(data))
				{
					return true;
				}
			}
			return false;
		}

		public T Get(int index)
		{
			LinkListNode<T> node = GetNode(index);
			return node != null ? node.value : default(T);
		}

		public int GetIndex(T data)
		{
			int index = 0;
			for(LinkListNode<T> current = first; current != null; current = current.next)
			{
				if(current.value.Equals(data))
				{
					return index;
				}
				++index;
			}
			return -1;
		}

		public bool SetIndex(T data, int index)
		{
			LinkListNode<T> node = GetNode(data);
			if(node == null)
				return false;

			TempRemove(node);
			TempAdd(node, index);
			return true;
		}

		public T Add(T data)
		{
			return Add(data, count);
		}

		public T Add(T data, int index)
		{
			if(!Contains(data))
			{
				LinkListNode<T> node;
				if(pool.Count > 0)
				{
					node = pool.Pop();
				}
				else
				{
					node = CreateNode();
				}
				node.nodelist = this;
				node.value = data;

				TempAdd(node, index);

				return data;
			}
			return default(T);
		}

		public T Remove(T data)
		{
			return RemoveNode(GetNode(data));
		}

		public T Remove(int index)
		{
			return RemoveNode(GetNode(index));
		}

		public bool Swap(int index1, int index2)
		{
			LinkListNode<T> node1 = GetNode(index1);
			LinkListNode<T> node2 = GetNode(index2);
			return Swap(node1, node2);
		}

		public bool Swap(T data1, T data2)
		{
			LinkListNode<T> node1 = GetNode(data1);
			LinkListNode<T> node2 = GetNode(data2);
			return Swap(node1, node2);
		}

		public bool Swap(ILinkListNode<T> nodeA, ILinkListNode<T> nodeB)
		{
			if(nodeA == null)
				return false;
			if(nodeB == null)
				return false;
			if(nodeA.NodeList != this)
				return false;
			if(nodeB.NodeList != this)
				return false;

			LinkListNode<T> node1 = (LinkListNode<T>)nodeA;
			LinkListNode<T> node2 = (LinkListNode<T>)nodeB;

			if(node1.previous == node2)
			{
				node1.previous = node2.previous;
				node2.previous = node1;
				node2.next = node1.next;
				node1.next = node2;
			}
			else if(node2.previous == node1)
			{
				node2.previous = node1.previous;
				node1.previous = node2;
				node1.next = node2.next;
				node2.next = node1;
			}
			else
			{
				LinkListNode<T> temp = node1.previous;
				node1.previous = node2.previous;
				node2.previous = temp;
				temp = node1.next;
				node1.next = node2.next;
				node2.next = temp;
			}
			if(first == node1)
			{
				first = node2;
			}
			else if(first == node2)
			{
				first = node1;
			}
			if(last == node1)
			{
				last = node2;
			}
			else if(last == node2)
			{
				last = node1;
			}
			if(node1.previous != null)
			{
				node1.previous.next = node1;
			}
			if(node2.previous != null)
			{
				node2.previous.next = node2;
			}
			if(node1.next != null)
			{
				node1.next.previous = node1;
			}
			if(node2.next != null)
			{
				node2.next.previous = node2;
			}
			return true;
		}

		private LinkListNode<T> GetNode(int index)
		{
			if(index < 0)
				return null;
			if(index > count - 1)
				return null;

			LinkListNode<T> current = first;

			while(index > 0)
			{
				current = current.next;
				--index;
			}
			return current;
		}

		private LinkListNode<T> GetNode(T data)
		{
			if(data != null)
			{
				for(LinkListNode<T> current = first; current != null; current = current.next)
				{
					if(current.value.Equals(data))
					{
						return current;
					}
				}
			}
			return null;
		}

		private T RemoveNode(LinkListNode<T> node)
		{
			if(node != null)
			{
				if(node.nodelist == this)
				{
					T data = node.value;

					node.nodelist = null;
					node.value = default(T);

					TempRemove(node);

					pool.Push(node);

					return data;
				}
			}
			return default(T);
		}

		private void TempRemove(LinkListNode<T> node)
		{
			if(node == first)
				first = first.next;
			if(node == last)
				last = last.previous;
			if(node.previous != null)
				node.previous.next = node.next;
			if(node.next != null)
				node.next.previous = node.previous;
			--count;
		}

		private void TempAdd(LinkListNode<T> node, int index)
		{
			if(first == null)
			{
				first = node;
				last = node;
				node.next = null;
				node.previous = null;
			}
			else
			{
				if(index <= 0)
				{
					node.next = first;
					node.previous = null;
					first.previous = node;
					first = node;
				}
				else if(index >= count)
				{
					last.next = node;
					node.previous = last;
					node.next = null;
					last = node;
				}
				else
				{
					LinkListNode<T> current = GetNode(index);
					node.next = current;
					node.previous = current.previous;
					current.previous.next = node;
					current.previous = node;
				}
			}
			++count;
		}

		override public string ToString()
		{
			string text = "[";
			if(first != null)
			{
				for(LinkListNode<T> current = first; current != null; current = current.next)
				{
					text += current.value + ", ";
				}
				text = text.Substring(0, text.Length - 2);
			}
			text += "]";
			return text;
		}
	}
}