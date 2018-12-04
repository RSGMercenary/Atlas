using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public abstract class Render2D : AtlasComponent, IRender2D
	{
		private int visible = 0;

		public Color Color { get; set; } = Color.White;
		public Rectangle? Crop { get; set; }
		public Vector2 Center { get; set; }
		public SpriteEffects Effects { get; set; } = SpriteEffects.None;

		public Render2D() { }

		public bool IsVisible
		{
			get { return visible > -1; }
			set
			{
				if(value)
					++visible;
				else
					--visible;
			}
		}

		public abstract void Draw(SpriteBatch batch, Vector2 scale, float rotation, Vector2 position, float layer);
	}
}