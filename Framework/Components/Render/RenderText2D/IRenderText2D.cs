using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public interface IRenderText2D : IRender2D
	{
		SpriteFont Font { get; set; }
		string Text { get; set; }
	}
}
