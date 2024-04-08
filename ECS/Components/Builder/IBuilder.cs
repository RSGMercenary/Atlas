using Atlas.ECS.Components.Component;

namespace Atlas.ECS.Components.Builder;

public interface IBuilder : IComponent<IBuilder>
{
	/// <summary>
	/// The current state of the build process. This can be
	/// unbuilt, building, or built.
	/// </summary>
	BuildState BuildState { get; }

	bool AutoRemove { get; set; }
}