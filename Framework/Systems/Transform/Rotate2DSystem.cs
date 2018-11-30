using Atlas.ECS.Systems;
using Atlas.Framework.Families.Render;

namespace Atlas.Framework.Systems.Transform
{
	public class Rotate2DSystem : AtlasFamilySystem<Rotate2DMember>
	{
		public Rotate2DSystem()
		{
			Priority = (int)SystemPriority.TestRotate2D;
		}

		protected override void MemberUpdate(float deltaTime, Rotate2DMember member)
		{
			member.Transform.Rotation += member.Rotate.Torque * deltaTime;
		}
	}
}
