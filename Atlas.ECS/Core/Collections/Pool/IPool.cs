using System;

namespace Atlas.Core.Collections.Pool;

public interface IPool : IDisposable
{
	int MaxCount { get; set; }

	int Count { get; }

	bool Fill();
	bool Empty();
}

public interface IPool<T> : IPool
{
	T Get();

	bool Put(T value);
}