using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public class RenderText2D : Render2D, IRenderText2D
	{
		public SpriteFont Font { get; set; }
		public string Text { get; set; }

		public RenderText2D() { }

		public RenderText2D(SpriteFont font, string text)
		{
			Font = font;
			Text = text;
		}

		public override void Draw(SpriteBatch batch, Vector2 scale, float rotation, Vector2 position, float layer)
		{
			batch.DrawString(Font, Text, position, Color, rotation, Center, scale, Effects, layer);
		}
	}
}
