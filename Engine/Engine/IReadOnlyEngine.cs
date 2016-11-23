using Atlas.Signals;

namespace Atlas.Engine.Engine
{
	interface IReadOnlyEngine<T>
	{
		/// <summary>
		/// The Engine managing this instance.
		/// </summary>
		IEngineManager Engine { get; }

		ISignal<T, IEngineManager, IEngineManager> EngineChanged { get; }
	}
}
