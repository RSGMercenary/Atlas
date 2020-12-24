using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool
{
	public class InstancePool<T> : Pool<T>
		where T : class, new()
	{
		public InstancePool() : base(() => new T()) { }
	}

	public class Pool<T> : IPool<T>
		where T : class
	{
		private readonly Stack<T> stack = new();
		private int maxCount = -1;
		private readonly Func<T> creator;

		public Pool(Func<T> creator)
		{
			this.creator = creator;
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

		public bool Release(T value)
		{
			if(value?.GetType() != typeof(T))
				throw new ArgumentException($"An instance of {value?.GetType()} does not equal {typeof(T)}.");
			if(maxCount >= 0 && stack.Count >= maxCount)
				return false;
			stack.Push(value);
			return true;
		}

		public bool Release(object value) => Release(value as T);

		public bool Fill()
		{
			if(maxCount <= 0)
				return false;
			if(stack.Count >= maxCount)
				return false;
			while(stack.Count < maxCount)
				Release(creator());
			return true;
		}

		#endregion

		#region Remove

		public T Get() => stack.Count > 0 ? stack.Pop() : creator();

		object IReadOnlyPool.Get() => Get();

		public bool Empty()
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