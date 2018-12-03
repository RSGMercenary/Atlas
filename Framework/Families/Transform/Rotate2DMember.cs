using Atlas.ECS.Families;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Render
{
	public class Rotate2DMember : AtlasFamilyMember
	{
		private ITransform2D transform;
		public ITransform2D Transform { get { return transform; } }

		private IRotate2D rotate;
		public IRotate2D Rotate { get { return rotate; } }
	}
}
