using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Components.EntityConstructor;

public interface IEntityConstructor : IComponent
{
	/// <summary>
	/// The <see langword="event"/> invoked when the <see cref="EntityConstructor.Construction"/> has changed.
	/// </summary>
	event Action<IEntityConstructor, Construction, Construction> ConstructionChanged;

	/// <summary>
	/// The <see cref="EntityConstructor.Construction"/> phase of the <see cref="IEntityConstructor"/>.
	/// </summary>
	Construction Construction { get; }

	bool AutoRemove { get; set; }
}

public interface IEntityConstructor<out T> : IEntityConstructor, IComponent<T> where T : IEntityConstructor
{
	/// <summary>
	/// The <see langword="event"/> invoked when the <see cref="EntityConstructor.Construction"/> has changed.
	/// </summary>
	new event Action<T, Construction, Construction> ConstructionChanged;
}