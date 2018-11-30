using Atlas.ECS.Families;
using Atlas.Framework.Components.Render;

namespace Atlas.Framework.Families.Render
{
	public class Render2DMember : AtlasFamilyMember
	{
		public IRender2D Render { get; set; }
	}
}
