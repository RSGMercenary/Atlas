using Atlas.Core.Objects;
using Atlas.ECS.Systems;
using Atlas.Framework.Families.Physics;

namespace Atlas.Framework.Systems.Physics
{
	public class AccelerationToVelocity2DSystem : AtlasFamilySystem<AccelerationToVelocity2DMember>
	{
		public AccelerationToVelocity2DSystem()
		{
			TimeStep = TimeStep.Fixed;
		}

		protected override void MemberUpdate(float deltaTime, AccelerationToVelocity2DMember member)
		{
			member.Velocity.Vector += member.Acceleration.Vector * deltaTime;
			member.Velocity.Rotation += member.Acceleration.Rotation * deltaTime;
		}
	}
}
