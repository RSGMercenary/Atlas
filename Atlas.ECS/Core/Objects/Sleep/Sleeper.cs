using System;
using System.Diagnostics;

namespace Atlas.Core.Objects.Sleep;

internal sealed class Sleeper<T> : ISleeper<T>, IDisposable
	where T : class, ISleeper
{
	#region Events
	public event Action<T, int, int> SleepingChanged;

	event Action<ISleeper, int, int> ISleeper.SleepingChanged
	{
		add => SleepingChanged += value;
		remove => SleepingChanged -= value;
	}
	#endregion

	#region Fields
	private readonly T Instance;
	private int sleeping = 0;
	#endregion

	public Sleeper(T instance)
	{
		Instance = instance;
	}

	public void Dispose()
	{
		SleepingChanged = null;
		sleeping = 0;
	}

	public int Sleeping
	{
		get => sleeping;
		private set
		{
			if(sleeping == value)
				return;
			var previous = sleeping;
			sleeping = value;
			Assert();
			SleepingChanged?.Invoke(Instance, value, previous);
		}
	}

	public bool IsSleeping => sleeping > 0;

	public void Sleep(bool sleep)
	{
		if(sleep)
			++Sleeping;
		else
			--Sleeping;
	}

	private void Assert()
	{
		if(sleeping < 0)
		{
			Trace.WriteLine($"{nameof(Sleep)}(false) was called more than {nameof(Sleep)}(true). {nameof(Sleeping)} should be >= 0, but was < 0.", TraceLevel.Warning.ToString());
			Trace.Write(new StackTrace(1));
		}
	}
}