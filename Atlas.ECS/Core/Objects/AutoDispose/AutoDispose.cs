using System;

namespace Atlas.Core.Objects.AutoDispose;

internal class AutoDispose<T> : IAutoDispose<T>
	where T : class, IAutoDispose<T>
{
	#region Events
	public event Action<T, bool> IsAutoDisposableChanged;

	event Action<IAutoDispose, bool> IAutoDispose.IsAutoDisposableChanged
	{
		add => IsAutoDisposableChanged += value;
		remove => IsAutoDisposableChanged -= value;
	}
	#endregion

	#region Fields
	private readonly T Instance;
	private readonly Func<bool> Condition;
	private bool isAutoDisposable = true;
	#endregion

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
			IsAutoDisposableChanged?.Invoke(Instance, value);
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