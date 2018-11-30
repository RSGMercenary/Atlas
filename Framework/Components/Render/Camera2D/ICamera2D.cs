using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Components.Render
{
	public interface ICamera2D : IComponent
	{
		IEntity Position { get; set; }
		IEntity Rotation { get; set; }
	}
}
