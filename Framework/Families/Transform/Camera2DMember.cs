using Atlas.ECS.Families;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Transform
{
	public class Camera2DMember : AtlasFamilyMember
	{
		private ITransform2D transform;
		public ITransform2D Transform { get { return transform; } }

		private ICamera2D camera;
		public ICamera2D Camera { get { return camera; } }
	}
}
