using Atlas.Core.Objects;
using Atlas.ECS.Systems;
using Atlas.Framework.Families.Physics;

namespace Atlas.Framework.Systems.Physics
{
	public class VelocityToTransform2DSystem : AtlasFamilySystem<VelocityToTransform2DMember>
	{
		public VelocityToTransform2DSystem()
		{
			TimeStep = TimeStep.Fixed;
			Priority = (int)SystemPriority.VelocityToTransform2D;
		}

		protected override void MemberUpdate(float deltaTime, VelocityToTransform2DMember member)
		{
			member.Transform.Position += member.Velocity.Vector * deltaTime;
			member.Transform.Rotation += member.Velocity.Rotation * deltaTime;
		}
	}
}
