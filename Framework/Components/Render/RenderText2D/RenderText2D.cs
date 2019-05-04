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
	}
}
