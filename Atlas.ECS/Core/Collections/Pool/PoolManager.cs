using Atlas.Core.Extensions;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool;

public sealed class PoolManager : IPoolManager
{
	public static readonly IPoolManager Instance = new PoolManager();

	public event Action<IPoolManager, IPool> PoolAdded;
	public event Action<IPoolManager, IPool> PoolRemoved;

	private readonly Dictionary<Type, uint> references = new();
	private readonly Dictionary<Type, IPool> pools = new();

	private PoolManager() { } // PoolManager can only have one instance.

	#region Pools
	#region Get
	public IReadOnlyDictionary<Type, IPool> Pools => pools;

	public IPool<T> GetPool<T>(Type type = null) => pools.TryGetValue(type ?? typeof(T), out IPool<T> pool) ? pool : null;

	public bool HasPool<T>() => GetPool<T>() != null;
	#endregion

	#region Add
	public IPool<T> AddPool<T>(int maxCount = -1, bool fill = false) where T : new() => AddPool(() => new T(), maxCount, fill);

	public IPool<T> AddPool<T>(Func<T> constructor = null, int maxCount = -1, bool fill = false)
	{
		var type = typeof(T);
		if(!pools.TryGetValue(type, out IPool<T> pool))
		{
			pool = new Pool<T>(constructor, maxCount, fill);
			pools.Add(type, pool);
			references.Add(type, 0);
			PoolAdded?.Invoke(this, pool);
		}
		++references[type];
		return pool;
	}
	#endregion

	#region Remove
	public bool RemovePool<T>()
	{
		var type = typeof(T);
		if(!pools.TryGetValue(type, out IPool<T> pool))
			return false;
		if(--references[type] > 0)
			return false;
		pool.Dispose();
		pools.Remove(type);
		references.Remove(type);
		PoolRemoved?.Invoke(this, pool);
		return true;
	}
	#endregion
	#endregion

	#region Instances
	public T Get<T>() => pools.TryGetValue(typeof(T), out IPool<T> pool) ? pool.Get() : Activator.CreateInstance<T>();

	public bool Put<T>(T value) => GetPool<T>(value.GetType())?.Put(value) ?? false;
	#endregion
}