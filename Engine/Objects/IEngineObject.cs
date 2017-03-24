using Atlas.Engine.Components;
using Atlas.Engine.Signals;

namespace Atlas.Engine
{
	interface IEngineObject<T>
	{
		IEngine Engine { get; set; }

		ISignal<T, IEngine, IEngine> EngineChanged { get; }

		bool Destroy();

		EngineObjectState State { get; }

		ISignal<T, EngineObjectState, EngineObjectState> StateChanged { get; }
	}
}
