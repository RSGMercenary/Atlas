using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Components.Builder;

public interface IBuilder : IComponent<IBuilder>
{
	event Action<IBuilder, BuildState, BuildState> BuildStateChanged;

	/// <summary>
	/// The current state of the build process. This can be
	/// unbuilt, building, or built.
	/// </summary>
	BuildState BuildState { get; }

	bool AutoRemove { get; set; }
}