using Atlas.Core.Collections.Pool;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Core.Collections.Group
{
	public class Group<T> : IGroup<T>
	{
		private class GroupItem
		{
			public T Value { get; set; }
			public bool IsRemoved { get; set; } = false;
		}

		private readonly List<GroupItem> items = new List<GroupItem>();
		private readonly Stack<GroupItem> removed = new Stack<GroupItem>();
		private readonly Pool<GroupItem> pool = new Pool<GroupItem>();
		private int iterators = 0;

		#region Pool

		private GroupItem GetItem(T value)
		{
			var item = pool.Remove();
			item.Value = value;
			item.IsRemoved = false;
			return item;
		}

		private void PoolItem(GroupItem element)
		{
			element.Value = default;
			//Leave IsRemoved = true for now. Seems cleaner.
			pool.Add(element);
		}

		#endregion

		#region Add

		public void Add(T value) => Insert(items.Count, value);

		public void Insert(int index, T value) => items.Insert(index, GetItem(value));

		#endregion

		#region Remove

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

		public void Clear()
		{
			items.Clear();
		}

		#endregion

		#region Get / Set

		public T this[int index]
		{
			get => items[index].Value;
			set
			{
				//TO-DO
				//I think this should be enough to handle
				//sorting algorithms swapping order during iteration.
				if(iterators > 0)
				{
					RemoveAt(index);
					Insert(index, value);
				}
				else
				{
					items[index].Value = value;
				}
			}
		}

		public int IndexOf(T value) => items.FindIndex(item => item.Value.Equals(value));

		public bool SetIndex(T item, int index)
		{
			var current = IndexOf(item);
			if(current == -1 || current == index)
				return false;
			RemoveAt(current);
			Insert(index, item);
			return true;
		}

		public bool Swap(T value1, T value2)
		{
			var index1 = IndexOf(value1);
			var index2 = IndexOf(value2);
			return Swap(index1, index2);
		}

		public bool Swap(int index1, int index2)
		{
			if(index1 < 0 || index2 < 0)
				return false;
			var item = items[index1];
			items[index1] = items[index2];
			items[index2] = item;
			return true;
		}

		#endregion

		#region Has

		public bool Contains(T item) => items.Find(element => element.Value.Equals(item)) != null;

		#endregion

		#region Enumerable

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<T> GetEnumerator() => Forward().GetEnumerator();

		public IEnumerable<T> Forward() => Enumerate(true);

		public IEnumerable<T> Backward() => Enumerate(false);

		private IEnumerable<T> Enumerate(bool forward)
		{
			if(items.Count <= 0)
				yield break;
			++iterators;
			var array = items.ToArray();
			var index = 0;
			var end = array.Length;
			if(!forward)
			{
				index = array.Length - 1;
				end = -1;
			}
			while(index != end)
			{
				if(!array[index].IsRemoved)
					yield return array[index].Value;
				index += forward ? 1 : -1;
			}
			if(--iterators == 0)
			{
				while(removed.Count > 0)
				{
					PoolItem(removed.Pop());
				}
			}
		}

		#endregion

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach(var item in items)
			{
				array[arrayIndex++] = item.Value;
			}
		}

		public int Count => items.Count;

		public bool IsReadOnly => false;

		public override string ToString()
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