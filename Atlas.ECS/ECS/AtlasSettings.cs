using Atlas.Core.Objects.AutoDispose;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;

namespace Atlas.ECS;

public static class AtlasSettings
{
	/// <summary>
	/// The <see cref="IAutoDisposer.AutoDispose"/> value used for all new <see cref="AtlasEntity"/> instances. The default is <see langword="true"/>.
	/// </summary>
	public static bool DefaultEntityAutoDispose { get; set; } = true;

	/// <summary>
	/// The <see cref="IAutoDisposer.AutoDispose"/> value used for all new <see cref="AtlasComponent"/> instances. The default is <see langword="true"/>.
	/// </summary>
	public static bool DefaultComponentAutoDispose { get; set; } = true;

	internal static void SetDefaultAutoDispose(IEntity entity) => entity.AutoDispose = DefaultEntityAutoDispose;

	internal static void SetDefaultAutoDispose(IComponent component) => component.AutoDispose = DefaultComponentAutoDispose;
}