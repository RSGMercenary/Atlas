using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Components.EntityConstructor;

public interface IEntityConstructor : IComponent
{
	event Action<IEntityConstructor, Construction, Construction> ConstructionStateChanged;

	/// <summary>
	/// The current state of the build process. This can be
	/// unbuilt, building, or built.
	/// </summary>
	Construction Construction { get; }

	bool AutoRemove { get; set; }
}

public interface IEntityConstructor<T> : IEntityConstructor, IComponent<T> where T : IEntityConstructor
{
	new event Action<T, Construction, Construction> ConstructionChanged;
}