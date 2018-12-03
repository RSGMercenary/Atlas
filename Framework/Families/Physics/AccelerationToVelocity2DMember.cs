using Atlas.ECS.Families;
using Atlas.Framework.Components.Physics;

namespace Atlas.Framework.Families.Physics
{
	public class AccelerationToVelocity2DMember : AtlasFamilyMember
	{
		public IAcceleration2D Acceleration;
		public IVelocity2D Velocity;
	}
}
