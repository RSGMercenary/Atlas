using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool
{
	public class Pool<T> : IPool<T>
		where T : class
	{
		private readonly Stack<T> stack = new Stack<T>();
		private int maxCount = -1;
		private readonly Func<T> creator;

		public Pool(Func<T> creator)
		{
			this.creator = creator;
		}

		public Pool(Func<T> creator, int maxCount) : this(creator)
		{
			MaxCount = maxCount;
		}

		#region Size

		public int Count
		{
			get { return stack.Count; }
		}

		public int MaxCount
		{
			get { return maxCount; }
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

		public bool Add(object value)
		{
			return Add(value as T);
		}

		public bool AddAll()
		{
			if(maxCount <= 0)
				return false;
			if(stack.Count >= maxCount)
				return false;
			while(stack.Count < maxCount)
				Add(creator.Invoke());
			return true;
		}

		#endregion

		#region Remove

		public T Remove()
		{
			return stack.Count > 0 ? stack.Pop() : creator.Invoke();
		}

		object IReadOnlyPool.Remove()
		{
			return Remove();
		}

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