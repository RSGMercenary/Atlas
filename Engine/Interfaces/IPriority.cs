using Atlas.Engine.Signals;

namespace Atlas.Interfaces
{
	interface IPriority<T>
	{
		int Priority { get; set; }

		Signal<T, int, int> PriorityChanged { get; }
	}
}
