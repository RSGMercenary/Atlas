using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public interface IRenderer2D : IComponent
	{
		SpriteBatch SpriteBatch { get; }
		Color BackgroundColor { get; set; }
	}
}
