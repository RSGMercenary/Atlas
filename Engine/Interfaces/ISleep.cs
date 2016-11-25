using Atlas.Engine.Signals;

namespace Atlas.Engine.Interfaces
{
	interface ISleep<T>:ISleepBase
	{
		ISignal<T, int, int> SleepingChanged { get; }
	}
}
