using Atlas.ECS.Components;

namespace Atlas.Framework.Components.Transform
{
	public interface IRotate2D : IComponent
	{
		float Torque { get; set; }
	}
}
