using Atlas.ECS.Families;
using Atlas.Framework.Components.Physics;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Physics
{
	public class VelocityToTransform2DMember : AtlasFamilyMember
	{
		private IVelocity2D velocity;
		public IVelocity2D Velocity { get { return velocity; } }

		private ITransform2D transform;
		public ITransform2D Transform { get { return transform; } }
	}
}
