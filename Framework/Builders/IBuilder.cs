using Atlas.Framework.Signals;
using System;

namespace Atlas.Framework.Builders
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

	public interface IBuilder<T> : IReadOnlyBuilder<T>
	{
		/// <summary>
		/// The current state of the build process. This can be
		/// unbuilt, building, or built.
		/// </summary>
		new BuildState BuildState { get; set; }

		void Built();

		bool AddBuilder(Action builder);
	}
}
