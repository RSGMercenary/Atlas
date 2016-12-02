using Atlas.Engine.Signals;

namespace Atlas.Engine.Interfaces
{
	interface ISleepHierarchy<T>:ISleepBase
	{
		ISignal<T, int, int, T> SleepingChanged { get; }
	}
}
