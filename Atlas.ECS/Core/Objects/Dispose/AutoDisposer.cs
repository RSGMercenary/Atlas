﻿using System;

namespace Atlas.Core.Objects.AutoDispose;

internal class AutoDisposer<T> : IAutoDisposer<T>
	where T : class, IAutoDisposer<T>
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
	private bool autoDispose = true;
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
		get => autoDispose;
		set
		{
			if(autoDispose == value)
				return;
			var previous = autoDispose;
			autoDispose = value;
			AutoDisposeChanged?.Invoke(Instance, value);
		}
	}

	public bool TryAutoDispose()
	{
		if(autoDispose && Condition())
		{
			Instance.Dispose();
			return true;
		}
		return false;
	}
}