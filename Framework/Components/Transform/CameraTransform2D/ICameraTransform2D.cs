using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public interface ICameraTransform2D : ITransform2D
	{
		Vector2 Center { get; set; }

		void Set(Vector2 position, float rotation, Vector2 scale, Vector2 center);
	}
}