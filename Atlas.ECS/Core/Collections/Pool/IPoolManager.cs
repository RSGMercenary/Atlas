using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool;

public interface IPoolManager
{
	event Action<IPoolManager, IPool> PoolAdded;
	event Action<IPoolManager, IPool> PoolRemoved;

	public IReadOnlyDictionary<Type, IPool> Pools { get; }

	public IPool<T> AddPool<T>(int maxCount = -1, bool fill = false) where T : new();

	public IPool<T> AddPool<T>(Func<T> creator, int maxCount = -1, bool fill = false);

	public bool RemovePool<T>();

	public IPool<T> GetPool<T>(Type type = null);

	public T Get<T>();

	public bool Put<T>(T value);
}