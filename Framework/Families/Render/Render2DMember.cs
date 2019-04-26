using Atlas.ECS.Families;
using Atlas.Framework.Components.Render;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Render
{
	public class Render2DMember : AtlasFamilyMember
	{
		public ITransform2D Transform { get; }
		public IRender2D Render { get; }
	}
}
