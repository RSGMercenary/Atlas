using Atlas.Core.Extensions;
using Atlas.Core.Messages;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool;

public sealed class PoolManager : Messenger<IPoolManager>, IPoolManager
{
	public static readonly IPoolManager Instance = new PoolManager();

	private readonly Dictionary<Type, uint> references = new();
	private readonly Dictionary<Type, IPool> pools = new();

	private PoolManager() { } // PoolManager can only have one instance.

	public override void Dispose() { } // PoolManager can't be disposed.

	#region Pools
	#region Get
	public IReadOnlyDictionary<Type, IPool> Pools => pools;

	public IPool<T> GetPool<T>(Type type = null) => pools.TryGetValue(type ?? typeof(T), out IPool<T> pool) ? pool : null;

	public bool ContainsPool<T>() => GetPool<T>() != null;
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
			Message<IPoolAddMessage>(new PoolAddMessage(type, pool));
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
		Message<IPoolRemoveMessage>(new PoolRemoveMessage(type, pool));
		return true;
	}
	#endregion
	#endregion

	#region Instances
	public T Get<T>() => pools.TryGetValue(typeof(T), out IPool<T> pool) ? pool.Get() : Activator.CreateInstance<T>();

	public bool Put<T>(T value) => GetPool<T>(value.GetType())?.Put(value) ?? false;
	#endregion
}