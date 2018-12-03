using Atlas.ECS.Components;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public interface ITransform2D : IComponent
	{
		Vector2 Position { get; set; }
		Vector2 Scale { get; set; }
		float Rotation { get; set; }

		void Set(Vector2 position, float rotation, Vector2 scale);

		float PositionX { get; set; }
		float PositionY { get; set; }
		float ScaleX { get; set; }
		float ScaleY { get; set; }
		float ScaleXY { get; set; }
	}
}
