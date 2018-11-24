namespace Atlas.Core.Collections.Pool
{
	public interface IReadOnlyPool
	{
		/// <summary>
		/// The capacity of the <see cref="IReadOnlyPool"/>. A capacity ==
		/// 0 makes the <see cref="IInstancePool{T}"/> hold an indeterminate amount of <see cref="{T}"/>.
		/// A capacity &lt; 0 prevents the <see cref="IInstancePool{T}"/> from pooling items, which can be used
		/// to enable/disable pooling even if code still calls for the <see cref="IInstancePool{T}"/>.
		/// </summary>
		int MaxCount { get; set; }

		int Count { get; }

		bool RemoveAll();
		object Remove();
	}

	public interface IReadOnlyPool<T> : IReadOnlyPool
		where T : class
	{
		new T Remove();
	}

	public interface IPool : IReadOnlyPool
	{
		bool AddAll();
		bool Add(object value);
	}

	public interface IPool<T> : IPool, IReadOnlyPool<T>
		where T : class
	{
		bool Add(T value);
	}
}
