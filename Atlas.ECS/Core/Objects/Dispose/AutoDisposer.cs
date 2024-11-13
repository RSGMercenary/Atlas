using System;

namespace Atlas.Core.Objects.AutoDispose;

internal sealed class AutoDisposer<T> : IAutoDisposer<T>, IDisposable
	where T : class, IAutoDisposer
{
	#region Events
	public event Action<T, bool> AutoDisposeChanged;

	event Action<IAutoDisposer, bool> IAutoDisposer.AutoDisposeChanged
	{
		add => AutoDisposeChanged += value;
		remove => AutoDisposeChanged -= value;
	}
	#endregion

	#region Fields
	private readonly T Instance;
	private readonly Func<bool> Condition;
	#endregion

	public AutoDisposer(T instance, Func<bool> condition)
	{
		Instance = instance;
		Condition = condition;
	}

	public void Dispose()
	{
		AutoDisposeChanged = null;
		AutoDispose = true;
	}

	public bool AutoDispose
	{
		get => field;
		set
		{
			if(field == value)
				return;
			var previous = field;
			field = value;
			AutoDisposeChanged?.Invoke(Instance, value);
		}
	}

	public bool TryAutoDispose()
	{
		if(AutoDispose && Condition())
		{
			Instance.Dispose();
			return true;
		}
		return false;
	}
}