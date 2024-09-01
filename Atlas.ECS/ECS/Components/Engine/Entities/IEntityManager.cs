using Atlas.Core.Collections.LinkList;
using Atlas.ECS.Entities;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Entities;

public interface IEntityManager : IReadOnlyEngineManager
{
	event Action<IEntityManager, IEntity> Added;

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