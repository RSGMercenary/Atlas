using Atlas.ECS.Components.Engine;
using System;

namespace Atlas.Core.Objects.Sleep;

internal class Sleep<T> : ISleep<T>, IDisposable
	where T : class, ISleep<T>, IEngineManager<T>
{
	#region Events
	public event Action<T, int, int> SleepingChanged;

	event Action<ISleep, int, int> ISleep.SleepingChanged
	{
		add => SleepingChanged += value;
		remove => SleepingChanged -= value;
	}
	#endregion

	#region Fields
	private readonly T Instance;
	private int sleeping = 0;
	#endregion

	public Sleep(T instance)
	{
		Instance = instance;
	}

	public void Dispose()
	{
		SleepingChanged = null;
		Sleeping = 0;
	}

	public int Sleeping
	{
		get => sleeping;
		set
		{
			if(sleeping == value)
				return;
			int previous = sleeping;
			sleeping = value;
			SleepingChanged?.Invoke(Instance, value, previous);
		}
	}

	public bool IsSleeping
	{
		get => sleeping > 0;
		set
		{
			if(value)
				++Sleeping;
			else
				--Sleeping;
		}
	}
}