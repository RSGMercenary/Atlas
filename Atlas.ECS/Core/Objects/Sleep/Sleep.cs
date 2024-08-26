using Atlas.ECS.Components.Engine;
using System;

namespace Atlas.Core.Objects.Sleep;

internal class Sleep<T> : ISleep<T>
	where T : ISleep<T>, IEngineManager<T>
{
	public event Action<T, int, int> SleepingChanged;
	private readonly T Instance;
	private int sleeping = 0;

	public Sleep(T instance)
	{
		Instance = instance;
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