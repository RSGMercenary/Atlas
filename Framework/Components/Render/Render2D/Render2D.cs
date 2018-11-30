using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Atlas.Framework.Components.Render
{
	public class Render2D : AtlasComponent, IRender2D
	{
		public Texture2D Texture { get; set; }
		public Vector2 Origin { get; set; }

		public Render2D(Texture2D texture)
		{
			Texture = texture;
		}

		public Render2D(Texture2D texture, Color fill)
		{
			Texture = texture;
			Fill(fill);
		}

		public Render2D(Texture2D texture, Vector2 origin, Color fill)
		{
			Texture = texture;
			Origin = origin;
			Fill(fill);
		}

		public void Fill(Color color)
		{
			var range = Texture.Width * Texture.Height;
			Texture.SetData(Enumerable.Range(0, range).Select(p => color).ToArray());
		}
	}
}
