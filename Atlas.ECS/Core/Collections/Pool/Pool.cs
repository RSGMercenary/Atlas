using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Pool;

public class Pool<T> : IPool<T>
{
	private readonly Stack<T> stack = new();
	private readonly Func<T> constructor;

	public Pool(Func<T> constructor = null, int maxCount = -1, bool fill = false)
	{
		this.constructor = constructor ?? Activator.CreateInstance<T>;
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
		get => field;
		set
		{
			if(field == value)
				return;
			field = value;
			if(field < 0)
				return;
			while(stack.Count > field)
				Dispose(stack.Pop());
		}
	}

	#endregion

	#region Add

	public bool Put(T value)
	{
		if(value?.GetType() != typeof(T))
			throw new ArgumentException($"An instance of {value?.GetType()} does not equal {typeof(T)}.");
		if(MaxCount >= 0 && stack.Count >= MaxCount)
			return false;
		stack.Push(value);
		return true;
	}

	public bool Fill()
	{
		if(MaxCount <= 0)
			return false;
		if(stack.Count >= MaxCount)
			return false;
		while(stack.Count < MaxCount)
			Put(constructor.Invoke());
		return true;
	}
	#endregion

	#region Remove
	public T Get() => stack.TryPop(out var value) ? value : constructor.Invoke();

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