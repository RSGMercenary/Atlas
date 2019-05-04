using Atlas.ECS.Components;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Render
{
	public abstract class Render2D : AtlasComponent, IRender2D
	{
		private int visible = 1;

		public Color Color { get; set; } = Color.White;

		public Rectangle? Crop { get; set; }
		public Vector2 Center { get; set; }

		public Render2D() { }

		public bool IsVisible
		{
			get { return visible > 0; }
			set
			{
				if(value)
					++visible;
				else
					--visible;
			}
		}

		public Color GlobalColor
		{
			get
			{
				var c = Color.ToVector4();
				var render = Manager?.GetAncestorComponent<IRender2D>();
				if(render != null)
					c *= render.GlobalColor.ToVector4();
				return new Color(c);
			}
		}
	}
}