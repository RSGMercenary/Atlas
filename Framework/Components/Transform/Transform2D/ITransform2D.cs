using Atlas.ECS.Components;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public interface ITransform2D : IComponent
	{
		Vector2 Position { get; set; }
		Vector2 Scale { get; set; }
		float Rotation { get; set; }
	}
}
