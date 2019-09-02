using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool
{
	public class Pool<T> : IPool<T>
		where T : class, new()
	{
		private readonly Stack<T> stack = new Stack<T>();
		private int maxCount = -1;

		public Pool() { }

		public Pool(int maxCount)
		{
			MaxCount = maxCount;
		}

		#region Size

		public int Count => stack.Count;

		public int MaxCount
		{
			get => maxCount;
			set
			{
				if(maxCount == value)
					return;
				maxCount = value;
				if(maxCount < 0)
					return;
				while(stack.Count > maxCount)
					stack.Pop();
			}
		}

		#endregion

		#region Add

		public bool Add(T value)
		{
			if(value == null)
				return false;
			if(typeof(T) != value.GetType())
				return false;
			if(maxCount >= 0 && stack.Count >= maxCount)
				return false;
			stack.Push(value);
			return true;
		}

		public bool Add(object value) => Add(value as T);

		public bool AddAll()
		{
			if(maxCount <= 0)
				return false;
			if(stack.Count >= maxCount)
				return false;
			while(stack.Count < maxCount)
				Add(new T());
			return true;
		}

		#endregion

		#region Remove

		public T Remove() => stack.Count > 0 ? stack.Pop() : new T();

		object IReadOnlyPool.Remove() => Remove();

		public bool RemoveAll()
		{
			if(stack.Count <= 0)
				return false;
			while(stack.Count > 0)
				stack.Pop();
			return true;
		}

		#endregion
	}
}