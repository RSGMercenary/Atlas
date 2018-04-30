using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Engine.Collections.EngineList
{
	public class EngineList<T> : IEngineList<T>
	{
		private class EngineListItem
		{
			public EngineListItem()
			{

			}

			public EngineListItem(T item)
			{
				Item = item;
			}

			public T Item { get; set; }
			public bool IsRemoved { get; set; } = false;

		}

		private readonly List<EngineListItem> list = new List<EngineListItem>();
		private Stack<EngineListItem> pooled = new Stack<EngineListItem>();
		private Stack<EngineListItem> removed = new Stack<EngineListItem>();
		private int iterators = 0;

		private EngineListItem GetItem(T item)
		{
			var element = pooled.Count > 0 ? pooled.Pop() : new EngineListItem();
			element.Item = item;
			element.IsRemoved = false;
			return element;
		}

		private void PoolItem(EngineListItem element)
		{
			element.Item = default(T);
			//Leave IsRemoved = true for now. Seems cleaner.
			pooled.Push(element);

		}

		public void Add(T item)
		{
			list.Add(GetItem(item));
		}

		public void Insert(int index, T item)
		{
			list.Insert(index, GetItem(item));
		}

		public bool Remove(T item)
		{
			var index = IndexOf(item);
			RemoveAt(index);
			return index > -1;
		}

		public void RemoveAt(int index)
		{
			if(index < 0 || index >= Count)
				return;
			var element = list[index];
			element.IsRemoved = true;
			list.RemoveAt(index);
			if(iterators > 0)
			{
				removed.Push(element);
			}
			else
			{
				PoolItem(element);
			}
		}

		public T this[int index]
		{
			get { return list[index].Item; }
			set { list[index].Item = value; }
		}

		public int IndexOf(T item)
		{
			return list.FindIndex(element => { return element.Item.Equals(item); });
		}

		public bool SetIndex(T item, int index)
		{
			var current = IndexOf(item);
			if(current == -1 || current == index)
				return false;
			RemoveAt(current);
			Insert(index, item);
			return true;
		}

		public void Clear()
		{
			list.Clear();
		}

		public bool Contains(T item)
		{
			return list.Find(element => { return element.Item.Equals(item); }) != null;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			//TO-DO
			//This might cause bugs if you just want a current state of the list.
			//!IsRemoved checks still apply during this iteration.
			foreach(var item in this)
			{
				array[arrayIndex++] = item;
			}
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Forward().GetEnumerator();
		}

		public IEnumerable<T> Forward()
		{
			return Enumerate(0, list.Count, 1);
		}

		public IEnumerable<T> Backward()
		{
			return Enumerate(list.Count - 1, -1, -1);
		}

		private IEnumerable<T> Enumerate(int index, int end, int direction)
		{
			++iterators;
			var items = list.ToArray();
			while(index != end)
			{
				if(!items[index].IsRemoved)
					yield return items[index].Item;
				index += direction;
			}
			if(--iterators == 0)
			{
				while(removed.Count > 0)
				{
					PoolItem(removed.Pop());
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Swap(T item1, T item2)
		{
			var index1 = IndexOf(item1);
			var index2 = IndexOf(item2);
			return Swap(index1, index2);
		}

		public bool Swap(int index1, int index2)
		{
			if(index2 < 0 || index2 < 0)
				return false;
			var temp = list[index1];
			list[index1] = list[index2];
			list[index2] = temp;
			return true;
		}

		public T Find(Func<T, bool> method)
		{
			foreach(var item in this)
				if(method(item))
					return item;
			return default(T);
		}

		override public string ToString()
		{
			StringBuilder text = new StringBuilder();
			text.Append('[');
			if(list.Count > 0)
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
	}
}
