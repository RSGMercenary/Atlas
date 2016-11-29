using Atlas.Engine.Signals;

namespace Atlas.Engine.Engine
{
	interface IReadOnlyEngineObject<T>
	{
		/// <summary>
		/// The Engine managing this instance.
		/// </summary>
		IEngine Engine { get; }

		ISignal<T, IEngine, IEngine> EngineChanged { get; }
	}
}
