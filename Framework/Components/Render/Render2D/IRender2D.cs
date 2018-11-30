using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework.Components.Render
{
	public interface IRender2D : IComponent
	{
		Texture2D Texture { get; set; }
		Vector2 Origin { get; set; }
	}
}
