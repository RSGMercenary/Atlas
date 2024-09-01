using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine.Entities;
using Atlas.ECS.Components.Engine.Families;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Components.Engine.Updates;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;

namespace Atlas.ECS.Components.Engine;

/// <summary>
/// An <see cref="IComponent"/> <see langword="interface"/> providing <see cref="IEntity"/>, <see cref="IFamily"/>, <see cref="ISystem"/>,
/// and fixed / variable <see cref="IUpdate{T}.Update(T)"/> loop management.
/// </summary>
public interface IEngine : IComponent<IEngine>
{
	/// <summary>
	/// The <see cref="IEntityManager"/> managing all <see cref="IEntity"/> instances in the <see cref="IEngine"/>.
	/// </summary>
	IEntityManager Entities { get; }

	/// <summary>
	/// The <see cref="IFamilyManager"/> managing all <see cref="IFamily"/> instances in the <see cref="IEngine"/>.
	/// </summary>
	IFamilyManager Families { get; }

	/// <summary>
	/// The <see cref="ISystemManager"/> managing all <see cref="ISystem"/> instances in the <see cref="IEngine"/>.
	/// </summary>
	ISystemManager Systems { get; }

	/// <summary>
	/// The <see cref="IUpdateManager"/> managing fixed / variable <see cref="IUpdate{T}.Update(T)"/> loops in the <see cref="IEngine"/>.
	/// </summary>
	IUpdateManager Updates { get; }
}