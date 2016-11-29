using Atlas.Engine.Signals;

namespace Atlas.Engine.Interfaces
{
	interface ISleep<T>
	{
		bool IsSleeping { get; }
		int Sleeping { get; set; }
		ISignal<T, int, int> SleepingChanged { get; }
	}
}
