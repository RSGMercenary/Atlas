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
	#endregion

	public Sleeper(T instance)
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
		get => field;
		private set
		{
			if(field == value)
				return;
			var previous = field;
			field = value;
			Assert();
			SleepingChanged?.Invoke(Instance, value, previous);
		}
	}

	public bool IsSleeping => Sleeping > 0;

	public void Sleep(bool sleep)
	{
		if(sleep)
			++Sleeping;
		else
			--Sleeping;
	}

	private void Assert()
	{
		if(Sleeping < 0)
		{
			Trace.WriteLine($"{nameof(Sleep)}(false) was called more than {nameof(Sleep)}(true). {nameof(Sleeping)} should be >= 0, but was < 0.", TraceLevel.Warning.ToString());
			Trace.Write(new StackTrace(1));
		}
	}
}