using Atlas.Core.Collections.LinkList;
using Atlas.ECS.Entities;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Entities;

/// <summary>
/// An <see langword="interface"/> providing <see cref="IEntity"/> management.
/// </summary>
public interface IEntityManager : IReadOnlyEngineManager
{
	/// <summary>
	/// The <see langword="event"/> invoked when an <see cref="IEntity"/> is added.
	/// </summary>
	event Action<IEntityManager, IEntity> Added;

	/// <summary>
	/// The <see langword="event"/> invoked when an <see cref="IEntity"/> is removed.
	/// </summary>
	event Action<IEntityManager, IEntity> Removed;

	/// /// <summary>
	/// All <see cref="IEntity"/> instances managed by the <see cref="IEntityManager"/>.
	/// <para><see cref="IEntity"/> instances are added/removed by becoming descendants of the <see cref="IEntity"/>.Root instance.</para>
	/// </summary>
	IReadOnlyLinkList<IEntity> Entities { get; }

	/// <summary>
	/// All <see cref="IEntity"/> instances by <see cref="IEntity.GlobalName"/> managed by the <see cref="IEntityManager"/>.
	/// </summary>
	IReadOnlyDictionary<string, IEntity> GlobalNames { get; }

	/// <summary>
	/// Determines if <see cref="IEntity"/> instances have their <see cref="IEntity.GlobalName"/> renamed if the name is already taken.
	/// <para>If <see langword="true"/>, the <see cref="IEntity"/> is renamed. If <see langword="false"/>, an <see cref="Exception"/> is thrown. Thr default is <see langword="true"/>.</para>
	/// </summary>
	bool RenameDuplicateGlobalNames { get; set; }

	/// <summary>
	/// Returns the <see cref="IEntity"/> with the given <see cref="IEntity.GlobalName"/>.
	/// </summary>
	/// <param name="globalName">The <see cref="IEntity.GlobalName"/>.</param>
	/// <returns></returns>
	IEntity Get(string globalName);

	/// <summary>
	/// Returns if the <see cref="IEntityManager"/> is managing an <see cref="IEntity"/> with the given <see cref="IEntity.GlobalName"/>.
	/// </summary>
	/// <param name="globalName">The <see cref="IEntity.GlobalName"/>.</param>
	/// <returns></returns>
	bool Has(string globalName);

	/// <summary>
	/// Returns if the <see cref="IEntityManager"/> is managing an <see cref="IEntity"/> with the given instance.
	/// </summary>
	/// <param name="entity">The <see cref="IEntity"/> instance.</param>
	/// <returns></returns>
	bool Has(IEntity entity);
}