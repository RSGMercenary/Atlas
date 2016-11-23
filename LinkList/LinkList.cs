using System.Collections.Generic;

namespace Atlas.LinkList
{
	class LinkList<T>:ILinkList<T>
	{
		private Stack<LinkListNode<T>> pooled = new Stack<LinkListNode<T>>();
		private Stack<LinkListNode<T>> removed = new Stack<LinkListNode<T>>();
		private LinkListNode<T> first;
		private LinkListNode<T> last;
		private int count = 0;

		bool isDisposed = false;

		public LinkList()
		{

		}

		public void Dispose()
		{
			if(!isDisposed)
			{
				IsDisposed = true;
				pooled.Clear();
				DisposeNodes();
			}
		}

		public bool IsDisposed
		{
			get
			{
				return isDisposed;
			}
			private set
			{
				if(isDisposed != value)
				{
					isDisposed = value;
				}
			}
		}

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
			if(data == null)
				return default(T);
			IsDisposed = false;
			LinkListNode<T> node;
			if(pooled.Count > 0)
			{
				node = pooled.Pop();
			}
			else
			{
				node = new LinkListNode<T>();
			}
			node.linklist = this;
			node.value = data;

			TempAdd(node, index);

			return data;
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

		public bool Swap(ILinkListNode<T> node1, ILinkListNode<T> node2)
		{
			if(node1 == null)
				return false;
			if(node2 == null)
				return false;
			if(node1.NodeList != this)
				return false;
			if(node2.NodeList != this)
				return false;

			LinkListNode<T> node01 = (LinkListNode<T>)node1;
			LinkListNode<T> node02 = (LinkListNode<T>)node2;

			if(node01.previous == node02)
			{
				node01.previous = node02.previous;
				node02.previous = node01;
				node02.next = node01.next;
				node01.next = node02;
			}
			else if(node02.previous == node01)
			{
				node02.previous = node01.previous;
				node01.previous = node02;
				node01.next = node02.next;
				node02.next = node01;
			}
			else
			{
				LinkListNode<T> temp = node01.previous;
				node01.previous = node02.previous;
				node02.previous = temp;
				temp = node01.next;
				node01.next = node02.next;
				node02.next = temp;
			}
			if(first == node01)
			{
				first = node02;
			}
			else if(first == node02)
			{
				first = node01;
			}
			if(last == node01)
			{
				last = node02;
			}
			else if(last == node02)
			{
				last = node01;
			}
			if(node01.previous != null)
			{
				node01.previous.next = node01;
			}
			if(node02.previous != null)
			{
				node02.previous.next = node02;
			}
			if(node01.next != null)
			{
				node01.next.previous = node01;
			}
			if(node02.next != null)
			{
				node02.next.previous = node02;
			}
			return true;
		}

		private LinkListNode<T> GetNode(int index)
		{
			if(index < 0)
				return null;
			if(index > count - 1)
				return null;

			if(index == 0)
				return first;
			if(index == count - 1)
				return last;
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
			if(count <= 0)
				return null;
			if(data == null)
				return null;
			if(first.value.Equals(data))
				return first;
			if(last.value.Equals(data))
				return last;
			for(LinkListNode<T> current = first.next; current != null && current != last; current = current.next)
			{
				if(current.value.Equals(data))
				{
					return current;
				}
			}
			return null;
		}

		private T RemoveNode(LinkListNode<T> node)
		{
			if(node == null)
				return default(T);
			if(node.linklist != this)
				return default(T);
			T data = node.value;
			node.linklist = null;
			node.value = default(T);
			TempRemove(node);
			removed.Push(node);
			return data;
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

		public void DisposeNodes()
		{
			while(removed.Count > 0)
			{
				DisposeNode(removed.Pop());
			}
		}

		private void DisposeNode(LinkListNode<T> node)
		{
			node.linklist = null;
			node.value = default(T);
			node.previous = null;
			node.next = null;
			if(!isDisposed)
				pooled.Push(node);
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

		public bool IsEmpty
		{
			get
			{
				return count <= 0;
			}
		}
	}
}