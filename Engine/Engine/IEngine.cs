using Atlas.Signals;

namespace Atlas.Engine.Engine
{
	interface IEngine<T>
	{
		/// <summary>
		/// The Engine managing this instance.
		/// </summary>
		IEngineManager Engine { get; set; }
		ISignal<T, IEngineManager, IEngineManager> EngineChanged { get; }
	}
}
