using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public interface IRenderer2D : IComponent
	{
		SpriteBatch Renderer { get; }
		Color Background { get; set; }
	}
}
