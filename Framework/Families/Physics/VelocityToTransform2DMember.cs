using Atlas.ECS.Families;
using Atlas.Framework.Components.Physics;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Physics
{
	public class VelocityToTransform2DMember : AtlasFamilyMember
	{
		public IVelocity2D Velocity { get; }
		public ITransform2D Transform { get; }
	}
}
