using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Core.Collections.EngineList
{
	public class EngineList<T> : IEngineList<T>
	{
		private class EngineListItem
		{
			public T Value { get; set; }
			public bool IsRemoved { get; set; } = false;
		}

		private List<EngineListItem> items = new List<EngineListItem>();
		private Stack<EngineListItem> pooled = new Stack<EngineListItem>();
		private Stack<EngineListItem> removed = new Stack<EngineListItem>();
		private int iterators = 0;

		private EngineListItem GetItem(T value)
		{
			var item = pooled.Count > 0 ? pooled.Pop() : new EngineListItem();
			item.Value = value;
			item.IsRemoved = false;
			return item;
		}

		private void PoolItem(EngineListItem element)
		{
			element.Value = default(T);
			//Leave IsRemoved = true for now. Seems cleaner.
			pooled.Push(element);

		}

		public void Add(T value)
		{
			items.Add(GetItem(value));
		}

		public void Insert(int index, T value)
		{
			items.Insert(index, GetItem(value));
		}

		public bool Remove(T value)
		{
			var index = IndexOf(value);
			RemoveAt(index);
			return index > -1;
		}

		public void RemoveAt(int index)
		{
			if(index < 0 || index >= Count)
				return;
			var item = items[index];
			item.IsRemoved = true;
			items.RemoveAt(index);
			if(iterators > 0)
			{
				removed.Push(item);
			}
			else
			{
				PoolItem(item);
			}
		}

		public T this[int index]
		{
			get { return items[index].Value; }
			set { items[index].Value = value; }
		}

		public int IndexOf(T value)
		{
			return items.FindIndex(item => { return item.Value.Equals(value); });
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
			items.Clear();
		}

		public bool Contains(T item)
		{
			return items.Find(element => { return element.Value.Equals(item); }) != null;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach(var item in items)
			{
				array[arrayIndex++] = item.Value;
			}
		}

		public int Count
		{
			get { return items.Count; }
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
			return Enumerate(1);
		}

		public IEnumerable<T> Backward()
		{
			return Enumerate(-1);
		}

		private IEnumerable<T> Enumerate(sbyte direction)
		{
			if(this.items.Count <= 0)
				yield break;
			++iterators;
			var items = this.items.ToArray();
			var index = 0;
			var end = items.Length;
			if(direction < 0)
			{
				index = items.Length - 1;
				end = -1;
			}
			while(index != end)
			{
				if(!items[index].IsRemoved)
					yield return items[index].Value;
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

		public bool Swap(T value1, T value2)
		{
			var index1 = IndexOf(value1);
			var index2 = IndexOf(value2);
			return Swap(index1, index2);
		}

		public bool Swap(int index1, int index2)
		{
			if(index2 < 0 || index2 < 0)
				return false;
			var item = items[index1];
			items[index1] = items[index2];
			items[index2] = item;
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
			var text = new StringBuilder();
			text.Append('[');
			if(items.Count > 0)
			{
				foreach(var item in items)
				{
					text.Append(item.Value);
					text.Append(", ");
				}
				text.Remove(text.Length - 2, 2);
			}
			text.Append(']');
			return text.ToString();
		}
	}
}
