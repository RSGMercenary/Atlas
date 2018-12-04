using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public interface IRender2D : IComponent
	{
		bool IsVisible { get; set; }
		Color Color { get; set; }
		Rectangle? Crop { get; set; }
		Vector2 Center { get; set; }
		SpriteEffects Effects { get; set; }

		void Draw(SpriteBatch batch, Vector2 scale, float rotation, Vector2 position, float layer);
	}
}
