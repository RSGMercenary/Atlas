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
	/// The <see cref="AtlasEntity.AutoDispose"/> value
	/// for all new <see cref="AtlasEntity"/> instances.
	/// <para>The default value is <see langword="true"/>.</para>
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
	/// <returns><see langword="true"/> if all <see cref="IPool"/> references are removed.
	/// Otherwise <see langword="false"/>.</returns>
	public static bool RemoveEntityPool() => PoolManager.Instance.RemovePool<AtlasEntity>();
	#endregion

	#region Components
	/// <summary>
	/// The <see cref="AtlasComponent{T}.AutoDispose"/> value
	/// for all new <see cref="AtlasComponent{T}"/> instances.
	/// <para>The default value is <see langword="true"/>.</para>
	/// </summary>
	public static bool ComponentAutoDispose { get; set; } = true;

	/// <summary>
	/// The <see cref="AtlasEntityConstructor{T}.AutoRemove"/> value
	/// for all new <see cref="AtlasEntityConstructor{T}"/> instances.
	/// <para>The default value is <see langword="true"/>.</para>
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
	/// <returns>The <see cref="AtlasComponent"/> <see cref="IPool"/>.</returns>
	public static IPool<T> AddComponentPool<T>(int maxCount = -1, bool fill = false) where T : IComponent, new() => PoolManager.Instance.AddPool<T>(maxCount, fill);

	/// <summary>
	/// Removes an <see cref="AtlasComponent"/> <see cref="IPool"/> reference.
	/// </summary>
	/// <returns><see langword="true"/> if all <see cref="IPool"/> references are removed.
	/// Otherwise <see langword="false"/>.</returns>
	public static bool RemoveComponentPool<T>() where T : IComponent => PoolManager.Instance.RemovePool<T>();
	#endregion

	#region Systems
	/// <summary>
	/// The <see cref="AtlasSystem.TimeStep"/> value
	/// for all new <see cref="AtlasSystem"/> instances.
	/// <para>The default value is <see cref="TimeStep.Variable"/>.</para>
	/// </summary>
	public static TimeStep TimeStep { get; set; } = TimeStep.Variable;

	/// <summary>
	/// The <see cref="AtlasFamilySystem{TFamilyMember}.IgnoreSleep"/> value
	/// for all new <see cref="AtlasFamilySystem{TFamilyMember}"/> instances.
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public static bool IgnoreSleep { get; set; } = false;
	#endregion
}