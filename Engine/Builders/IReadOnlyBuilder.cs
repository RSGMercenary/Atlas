using Atlas.Engine.Signals;

namespace Atlas.Engine.Builders
{
	public interface IReadOnlyBuilder<T>
	{
		/// <summary>
		/// The current state of the build process. This can be
		/// unbuilt, building, or built.
		/// </summary>
		BuildState BuildState { get; }

		/// <summary>
		/// Signal dispatching when the BuildState has changed.
		/// </summary>
		ISignal<T, BuildState, BuildState> BuildStateChanged { get; }
	}
}
