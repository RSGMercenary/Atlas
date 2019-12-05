namespace Atlas.Core.Collections.Pool
{
	public interface IReadOnlyPool
	{
		int MaxCount { get; set; }

		int Count { get; }

		bool Empty();
		object Get();
	}

	public interface IReadOnlyPool<out T> : IReadOnlyPool
		where T : class
	{
		new T Get();
	}

	public interface IPool : IReadOnlyPool
	{
		bool Fill();
		bool Release(object value);
	}

	public interface IPool<T> : IPool, IReadOnlyPool<T>
		where T : class
	{
		bool Release(T value);
	}
}