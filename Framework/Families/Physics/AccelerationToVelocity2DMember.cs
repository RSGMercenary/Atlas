using Atlas.ECS.Families;
using Atlas.Framework.Components.Physics;

namespace Atlas.Framework.Families.Physics
{
	public class AccelerationToVelocity2DMember : AtlasFamilyMember
	{
		public IVelocity2D Velocity { get; set; }
		public IAcceleration2D Acceleration { get; set; }
	}
}
