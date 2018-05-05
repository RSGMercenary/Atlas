using System;
using System.Collections.Generic;

namespace Atlas.Framework.Pools
{
	public static class Pool
	{
		private static Dictionary<Type, IPool> pools = new Dictionary<Type, IPool>();

		public static bool Has<T>()
		{
			return pools.ContainsKey(typeof(T));
		}

		public static T GetOrDefault<T>()
		{
			var type = typeof(T);
			if(pools.ContainsKey(type))
				return (pools[type] as IPool<T>).Remove();
			return default(T);
		}

		public static T GetOrNew<T>() where T : new()
		{
			var type = typeof(T);
			if(pools.ContainsKey(type))
				return (pools[type] as IPool<T>).Remove();
			return new T();
		}

		public static IReadOnlyPool<T> Get<T>(IPool<T> pool)
		{
			var type = typeof(T);
			if(!pools.ContainsKey(type))
				pools.Add(type, pool);
			return pools[type] as IReadOnlyPool<T>;
		}

		public static IReadOnlyPool<T> Get<T>() where T : new()
		{
			var type = typeof(T);
			if(!pools.ContainsKey(type))
				pools.Add(type, new Pool<T>(() => new T()));
			return pools[type] as IReadOnlyPool<T>;
		}

		public static IPool<T> Get<T>(Func<T> creator) where T : class
		{
			var type = typeof(T);
			if(!pools.ContainsKey(type))
				pools.Add(type, new Pool<T>(creator));
			return pools[type] as IPool<T>;
		}
	}

	public class Pool<T> : IPool<T>
	{
		private readonly Stack<T> stack = new Stack<T>();
		private int maxCount = -1;
		private Func<T> creator;
		private Action<T> onRemove;
		private Func<T, bool> onAdd;

		public Pool(Func<T> creator)
		{
			this.creator = creator;
		}

		public Pool(Func<T> creator, Action<T> onRemove = null, Func<T, bool> onAdd = null) : this(creator)
		{
			this.onRemove = onRemove;
			this.onAdd = onAdd;
		}

		public Pool(Func<T> creator, int maxCount) : this(creator)
		{
			MaxCount = maxCount;
		}

		public int Count
		{
			get { return stack.Count; }
		}

		public bool AddAll()
		{
			if(maxCount <= 0)
				return false;
			if(stack.Count >= maxCount)
				return false;
			while(stack.Count < maxCount)
				if(!Add(creator.Invoke()))
					return false;
			return true;
		}

		public bool RemoveAll()
		{
			if(stack.Count <= 0)
				return false;
			while(stack.Count > 0)
				stack.Pop();
			return true;
		}

		public bool Add(T value)
		{
			if(value == null)
				return false;
			if(typeof(T) != value.GetType())
				return false;
			if(maxCount >= 0 && stack.Count >= maxCount)
				return false;
			if(onAdd != null && !onAdd.Invoke(value))
				return false;
			stack.Push(value);
			return true;
		}

		public T Remove()
		{
			var value = stack.Count > 0 ? stack.Pop() : creator.Invoke();
			onRemove?.Invoke(value);
			return value;
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

		public bool Add(object value)
		{
			return Add((T)value);
		}

		object IReadOnlyPool.Remove()
		{
			return Remove();
		}
	}
}
