using Atlas.ECS.Families;
using Atlas.Framework.Components.Physics;

namespace Atlas.Framework.Families.Physics
{
	public class AccelerationToVelocity2DMember : AtlasFamilyMember
	{
		private IAcceleration2D acceleration;
		public IAcceleration2D Acceleration { get { return acceleration; } }

		private IVelocity2D velocity;
		public IVelocity2D Velocity { get { return velocity; } }
	}
}
