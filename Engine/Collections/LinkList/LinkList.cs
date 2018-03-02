using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Engine.Collections.LinkList
{
	class LinkList<T> : ILinkList<T>
	{
		private Stack<LinkListNode<T>> pooled = new Stack<LinkListNode<T>>();
		private Stack<LinkListNode<T>> removed = new Stack<LinkListNode<T>>();
		private LinkListNode<T> first;
		private LinkListNode<T> last;
		private int count = 0;
		private int iterators = 0;

		bool isDisposed = false;

		public LinkList()
		{

		}

		public void Dispose()
		{
			if(!isDisposed)
			{
				IsDisposed = true;
				Clear();
				pooled.Clear();
			}
		}

		public bool IsDisposed
		{
			get { return isDisposed; }
			private set
			{
				if(isDisposed == value)
					return;
				isDisposed = value;
			}
		}

		public T this[int i]
		{
			get
			{
				return i >= 0 && i < count ? GetNode(i).Value : default(T);
			}
			set
			{
				if(i >= 0 && i < count)
					GetNode(i).value = value;
			}
		}

		public ILinkListNode<T> First
		{
			get { return first; }
		}

		public ILinkListNode<T> Last
		{
			get { return last; }
		}

		public int Count
		{
			get { return count; }
		}

		public bool Contains(T item)
		{
			if(item == null)
				return false;
			foreach(var current in this)
			{
				if(current.Equals(item))
					return true;
			}
			return false;
		}

		public T Get(int index)
		{
			var node = GetNode(index);
			return node != null ? node.Value : default(T);
		}

		public int GetIndex(T item)
		{
			if(item == null)
				return -1;
			int index = 0;
			foreach(var current in this)
			{
				if(current.Equals(item))
					return index;
				++index;
			}
			return -1;
		}

		public bool SetIndex(T item, int index)
		{
			var node = GetNode(item);
			if(node == null)
				return false;
			TempRemove(node);
			TempAdd(node, index);
			return true;
		}

		public void Add(params T[] items)
		{
			foreach(var item in items)
				Add(item);
		}

		public T Add(T item)
		{
			return Add(item, count);
		}

		public T Add(T item, int index)
		{
			if(item == null)
				return default(T);
			IsDisposed = false;
			var node = default(LinkListNode<T>);
			if(pooled.Count > 0)
			{
				node = pooled.Pop();
			}
			else
			{
				node = new LinkListNode<T>();
			}
			node.nodelist = this;
			node.value = item;

			TempAdd(node, index);

			return item;
		}

		public void Remove(params T[] items)
		{
			foreach(var item in items)
				Remove(item);
		}

		public T Remove(T item)
		{
			return RemoveNode(GetNode(item));
		}

		public T Remove(int index)
		{
			return RemoveNode(GetNode(index));
		}

		public bool Swap(int index1, int index2)
		{
			var node1 = GetNode(index1);
			var node2 = GetNode(index2);
			return Swap(node1, node2);
		}

		public bool Swap(T item1, T item2)
		{
			var node1 = GetNode(item1);
			var node2 = GetNode(item2);
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
			Swap(node1 as LinkListNode<T>, node2 as LinkListNode<T>);
			return true;
		}

		private bool Swap(LinkListNode<T> node1, LinkListNode<T> node2)
		{
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
				var temp = node1.previous;
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
			else if(First == node2)
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
			if(node1.previous)
			{
				node1.previous.next = node1;
			}
			if(node2.previous)
			{
				node2.previous.next = node2;
			}
			if(node1.next)
			{
				node1.next.previous = node1;
			}
			if(node2.next)
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

			if(index == 0)
				return first;
			if(index == count - 1)
				return last;
			var current = first;
			while(index > 0)
			{
				current = current.next;
				--index;
			}
			return current;
		}

		private LinkListNode<T> GetNode(T item)
		{
			if(count <= 0)
				return null;
			if(item == null)
				return null;
			var current = first;
			while(current != null)
			{
				if(current.value.Equals(item))
					return current;
				current = current.next;
			}
			return null;
		}

		private T RemoveNode(LinkListNode<T> node)
		{
			if(node == null)
				return default(T);
			T item = node.value;
			TempRemove(node);
			//Enumerators could be on 'current' at the time of removal.
			//Make sure 'current' doesn't point at another node being removed.
			foreach(var current in removed)
			{
				if(current.next == node)
				{
					current.next = node.next;
				}
				if(current.previous == node)
				{
					current.previous = node.previous;
				}
			}
			if(iterators > 0)
			{
				removed.Push(node);
			}
			else
			{
				DisposeNode(node);
			}
			return item;
		}

		private void TempRemove(LinkListNode<T> node)
		{

			if(node == first)
				first = first.next;
			if(node == last)
				last = last.previous;
			if(node.previous)
				node.previous.next = node.next;
			if(node.next)
				node.next.previous = node.previous;
			--count;
		}

		private void TempAdd(LinkListNode<T> node, int index)
		{
			if(!first)
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
					var current = GetNode(index);
					node.next = current;
					node.previous = current.previous;
					current.previous.next = node;
					current.previous = node;
				}
			}
			++count;
		}

		private void DisposeNodes()
		{
			while(removed.Count > 0)
			{
				DisposeNode(removed.Pop());
			}
		}

		private void DisposeNode(LinkListNode<T> node)
		{
			node.nodelist = null;
			node.value = default(T);
			if(node.previous)
				node.previous.next = node.next;
			if(node.next)
				node.next.previous = node.previous;
			node.previous = null;
			node.next = null;

			if(!isDisposed)
				pooled.Push(node);
		}

		override public string ToString()
		{
			StringBuilder text = new StringBuilder();
			text.Append('[');
			if(First != null)
			{
				foreach(var item in this)
				{
					text.Append(item);
					text.Append(", ");
				}
				text.Remove(text.Length - 2, 2);
			}
			text.Append(']');
			return text.ToString();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Forward().GetEnumerator();
		}

		public IEnumerable<T> Forward()
		{
			IterateStart();
			var current = first;
			while(current != null)
			{
				yield return current.value;
				current = current.next;
			}
			IterateStop();
		}

		public IEnumerable<T> Backward()
		{
			IterateStart();
			var current = last;
			while(current != null)
			{
				yield return current.value;
				current = current.previous;
			}
			IterateStop();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void IterateStart()
		{
			++iterators;
		}

		public void IterateStop()
		{
			if(iterators <= 0)
				return;
			if(--iterators == 0)
			{
				DisposeNodes();
			}
		}

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		public void Clear()
		{
			while(first)
			{
				RemoveNode(last);
			}
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach(T item in this)
			{
				array[arrayIndex++] = item;
			}
		}

		bool ICollection<T>.Remove(T item)
		{
			return Remove(item) != null;
		}

		public bool IsEmpty
		{
			get
			{
				return count <= 0;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public T Get(Func<T, bool> method)
		{
			foreach(var item in this)
				if(method(item))
					return item;
			return default(T);
		}
	}
}