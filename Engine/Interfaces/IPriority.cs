using Atlas.Engine.Signals;

namespace Atlas.Engine.Interfaces
{
	interface IPriority<T>
	{
		int Priority { get; set; }

		ISignal<T, int, int> PriorityChanged { get; }
	}
}
