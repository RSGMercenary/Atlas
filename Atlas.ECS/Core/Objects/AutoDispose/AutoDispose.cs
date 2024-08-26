using System;

namespace Atlas.Core.Objects.AutoDispose;

internal class AutoDispose<T> : IAutoDispose<T>, IDisposable
	where T : IAutoDispose<T>, IDisposable
{
	public event Action<T, bool, bool> IsAutoDisposableChanged;
	private readonly T Instance;
	private readonly Func<bool> Condition;
	private bool isAutoDisposable = true;

	public AutoDispose(T instance, Func<bool> condition)
	{
		Instance = instance;
		Condition = condition;
	}

	public void Dispose()
	{
		IsAutoDisposableChanged = null;
		IsAutoDisposable = true;
	}

	public bool IsAutoDisposable
	{
		get => isAutoDisposable;
		set
		{
			if(isAutoDisposable == value)
				return;
			var previous = isAutoDisposable;
			isAutoDisposable = value;
			IsAutoDisposableChanged?.Invoke(Instance, value, previous);
		}
	}

	public bool TryAutoDispose()
	{
		if(isAutoDisposable && Condition())
		{
			Instance.Dispose();
			return true;
		}
		return false;
	}
}