using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Physics
{
	public interface IVelocity2D
	{
		Vector2 Vector { get; set; }
		float Rotation { get; set; }
	}
}
