using Atlas.Core.Collections.Group;
using Atlas.ECS.Entities;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Entities;

public interface IEntityManager : IReadOnlyEngineManager
{
	event Action<IEntityManager, IEntity> Added;

	event Action<IEntityManager, IEntity> Removed;

	/// <summary>
	/// A collection of all Entities managed by this Engine.
	/// <para>Entities are added to and removed from the Engine by being
	/// added as a child of an Entity already in the Entity hierarchy.
	/// This is done by creating a root Entity, adding an Engine Component
	/// to it, and then adding children to that hierarchy.</para>
	/// </summary>
	IReadOnlyGroup<IEntity> Entities { get; }

	IReadOnlyDictionary<string, IEntity> GlobalNames { get; }

	/// <summary>
	/// Returns if the Engine is managing an Entity with the given global name.
	/// Every Entity has its own unique global name that can't be duplicated.
	/// </summary>
	/// <param name="globalName">The global name of the Entity.</param>
	/// <returns></returns>
	bool Has(string globalName);

	/// <summary>
	/// Returns if the Engine is managing an Entity with the given instance.
	/// </summary>
	/// <param name="entity"></param>
	/// <returns></returns>
	bool Has(IEntity entity);

	/// <summary>
	/// Returns the Entity with the given global name.
	/// Every Entity has its own unique global name that can't be duplicated.
	/// </summary>
	/// <param name="globalName"></param>
	/// <returns></returns>
	IEntity Get(string globalName);
}