using Atlas.ECS.Families;
using Atlas.Framework.Components.Render;

namespace Atlas.Framework.Families.Render
{
	public class Renderer2DMember : AtlasFamilyMember
	{
		public IRenderer2D Renderer { get; set; }
	}
}
