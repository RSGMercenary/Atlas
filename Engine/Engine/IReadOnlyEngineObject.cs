using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;

namespace Atlas.Engine.Engine
{
	interface IReadOnlyEngineObject<T>:IDispose<T>, IAutoDispose
	{
		IEngine Engine { get; }

		ISignal<T, IEngine, IEngine> EngineChanged { get; }
	}
}
