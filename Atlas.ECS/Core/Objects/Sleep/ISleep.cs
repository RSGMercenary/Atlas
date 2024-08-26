using System;

namespace Atlas.Core.Objects.Sleep;

public interface ISleep<T> where T : ISleep<T>
{
	event Action<T, int, int> SleepingChanged;
	bool IsSleeping { get; set; }
	int Sleeping { get; }
}