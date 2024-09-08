using Atlas.Core.Collections.Pool;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using Atlas.ECS.Systems;
using System.Diagnostics.CodeAnalysis;

namespace Atlas.ECS;

[ExcludeFromCodeCoverage]
public static class AtlasECS
{
	#region Entities
	/// <summary>
	/// The <see cref="IEntity.AutoDispose"/> value used for all new <see cref="AtlasEntity"/> instances. The default is <see langword="true"/>.
	/// </summary>
	public static bool EntityAutoDispose { get; set; } = true;

	public static IPool<AtlasEntity> AddEntityPool(int maxCount = -1, bool fill = false) => PoolManager.Instance.AddPool<AtlasEntity>(maxCount, fill);

	public static bool RemoveEntityPool() => PoolManager.Instance.RemovePool<AtlasEntity>();
	#endregion

	#region Components
	/// <summary>
	/// The <see cref="IComponent.AutoDispose"/> value used for all new <see cref="AtlasComponent"/> instances. The default is <see langword="true"/>.
	/// </summary>
	public static bool ComponentAutoDispose { get; set; } = true;

	public static bool AutoRemove { get; set; } = true;

	public static IPool<T> AddComponentPool<T>(int maxCount = -1, bool fill = false) where T : IComponent, new() => PoolManager.Instance.AddPool<T>(maxCount, fill);

	public static bool RemoveComponentPool<T>() where T : IComponent => PoolManager.Instance.RemovePool<T>();
	#endregion

	#region Systems
	/// <summary>
	/// The <see cref="AtlasSystem.TimeStep"/> value used for all new <see cref="AtlasSystem"/> instances. The default is <see cref="TimeStep.Variable"/>.
	/// </summary>
	public static TimeStep TimeStep { get; set; } = TimeStep.Variable;

	/// <summary>
	/// The <see cref="AtlasFamilySystem"/>.UpdateSleepingEntities value used for all new <see cref="AtlasFamilySystem"/> instances. The default is <see langword="false"/>.
	/// </summary>
	public static bool IgnoreSleep { get; set; } = false;
	#endregion
}