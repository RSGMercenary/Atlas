using Atlas.ECS.Families;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Render
{
	public class Rotate2DMember : AtlasFamilyMember
	{
		public ITransform2D Transform;
		public IRotate2D Rotate;
	}
}
