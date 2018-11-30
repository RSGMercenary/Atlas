using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Physics
{
	public interface IAcceleration2D
	{
		Vector2 Vector { get; set; }
		float Rotation { get; set; }
	}
}
