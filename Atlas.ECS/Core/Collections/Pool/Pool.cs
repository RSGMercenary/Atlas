using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool;

public sealed class Pool<T> : IPool<T>
{
	private readonly Stack<T> stack = new();
	private int maxCount = -1;
	private readonly Func<T> creator;

	public Pool(Func<T> creator, int maxCount = -1, bool fill = false)
	{
		if(creator == null)
			throw new ArgumentNullException(nameof(creator));
		this.creator = creator;
		MaxCount = maxCount;
		if(fill) Fill();
	}

	public void Dispose()
	{
		Empty();
	}

	private static void Dispose(T value)
	{
		if(value is IDisposable disposable)
			disposable.Dispose();
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
				Dispose(stack.Pop());
		}
	}

	#endregion

	#region Add

	public bool Put(T value)
	{
		if(value?.GetType() != typeof(T))
			throw new ArgumentException($"An instance of {value?.GetType()} does not equal {typeof(T)}.");
		if(maxCount >= 0 && stack.Count >= maxCount)
			return false;
		stack.Push(value);
		return true;
	}

	public bool Fill()
	{
		if(maxCount <= 0)
			return false;
		if(stack.Count >= maxCount)
			return false;
		while(stack.Count < maxCount)
			Put(creator.Invoke());
		return true;
	}
	#endregion

	#region Remove
	public T Get() => stack.TryPop(out var value) ? value : creator.Invoke();

	public bool Empty()
	{
		if(stack.Count <= 0)
			return false;
		while(stack.TryPop(out var value))
			Dispose(value);
		return true;
	}
	#endregion
}