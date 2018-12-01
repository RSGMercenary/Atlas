using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public class Renderer2D : AtlasComponent, IRenderer2D
	{
		public SpriteBatch SpriteBatch { get; private set; }
		public Color BackgroundColor { get; set; } = Color.Black;

		public Renderer2D(GraphicsDevice graphics)
		{
			SpriteBatch = new SpriteBatch(graphics);
		}
	}
}
