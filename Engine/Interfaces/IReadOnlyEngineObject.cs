using Atlas.Engine.Components;
using Atlas.Engine.Signals;

namespace Atlas.Engine.Interfaces
{
	interface IReadOnlyEngineObject<T> : IDispose<T>, IAutoDispose
	{
		IEngine Engine { get; }

		ISignal<T, IEngine, IEngine> EngineChanged { get; }
	}
}
