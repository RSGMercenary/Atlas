using Atlas.Core.Collections.Pool;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.EntityConstructor;
using Atlas.ECS.Entities;
using Atlas.ECS.Systems;
using System.Diagnostics.CodeAnalysis;

namespace Atlas.ECS;

[ExcludeFromCodeCoverage]
public static class AtlasECS
{
	#region Entities
	/// <summary>
	/// The default <see langword="true"/> value of <see cref="IEntity.AutoDispose"/> for all new <see cref="AtlasEntity"/> instances.
	/// </summary>
	public static bool EntityAutoDispose { get; set; } = true;

	/// <summary>
	/// Gets the <see cref="AtlasEntity"/> <see cref="IPool"/>.
	/// </summary>
	/// <returns>The <see cref="AtlasEntity"/> <see cref="IPool"/>.</returns>
	public static IPool<AtlasEntity> GetEntityPool() => PoolManager.Instance.GetPool<AtlasEntity>();

	/// <summary>
	/// Adds an <see cref="AtlasEntity"/> <see cref="IPool"/> reference.
	/// </summary>
	/// <param name="maxCount">The max instances in the pool.</param>
	/// <param name="fill">Determines if the pool starts filled.</param>
	/// <returns>The <see cref="AtlasEntity"/> <see cref="IPool"/>.</returns>
	public static IPool<AtlasEntity> AddEntityPool(int maxCount = -1, bool fill = false) => PoolManager.Instance.AddPool<AtlasEntity>(maxCount, fill);

	/// <summary>
	/// Removes an <see cref="AtlasEntity"/> <see cref="IPool"/> reference.
	/// </summary>
	/// <returns><see langword="true"/> if all <see cref="IPool"/> references are removed. Otherwise <see langword="false"/>.</returns>
	public static bool RemoveEntityPool() => PoolManager.Instance.RemovePool<AtlasEntity>();
	#endregion

	#region Components
	/// <summary>
	/// The default <see langword="true"/> value of <see cref="IComponent.AutoDispose"/> for all new <see cref="AtlasComponent"/> instances.
	/// </summary>
	public static bool ComponentAutoDispose { get; set; } = true;

	/// <summary>
	/// The default <see langword="true"/> value of <see cref="AtlasEntityConstructor"/>.AutoRemove for all new <see cref="AtlasEntityConstructor"/> instances.
	/// </summary>
	public static bool AutoRemove { get; set; } = true;

	/// <summary>
	/// Gets the <see cref="AtlasComponent"/> <see cref="IPool"/>.
	/// </summary>
	/// <returns>The <see cref="AtlasComponent"/> <see cref="IPool"/>.</returns>
	public static IPool<T> GetComponentPool<T>() where T : IComponent => PoolManager.Instance.GetPool<T>();

	/// <summary>
	/// Adds an <see cref="AtlasComponent"/> <see cref="IPool"/> reference.
	/// </summary>
	/// <param name="maxCount">The max instances in the pool.</param>
	/// <param name="fill">Determines if the pool starts filled.</param>
	/// <returns>The <see cref="AtlasComponent"/> pool.</returns>
	public static IPool<T> AddComponentPool<T>(int maxCount = -1, bool fill = false) where T : IComponent, new() => PoolManager.Instance.AddPool<T>(maxCount, fill);

	/// <summary>
	/// Removes an <see cref="AtlasComponent"/> <see cref="IPool"/> reference.
	/// </summary>
	/// <returns><see langword="true"/> if all <see cref="IPool"/> references are removed. Otherwise <see langword="false"/>.</returns>
	public static bool RemoveComponentPool<T>() where T : IComponent => PoolManager.Instance.RemovePool<T>();
	#endregion

	#region Systems
	/// <summary>
	/// The default <see cref="TimeStep.Variable"/> value of <see cref="AtlasSystem.TimeStep"/> for all new <see cref="AtlasSystem"/> instances.
	/// </summary>
	public static TimeStep TimeStep { get; set; } = TimeStep.Variable;

	/// <summary>
	/// The default <see langword="false"/> value of <see cref="AtlasFamilySystem"/>.UpdateSleepingEntities for all new <see cref="AtlasFamilySystem"/> instances.
	/// </summary>
	public static bool IgnoreSleep { get; set; } = false;
	#endregion
}