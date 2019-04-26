using Atlas.ECS.Families;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Transform
{
	public class Camera2DMember : AtlasFamilyMember
	{
		public ITransform2D Transform { get; }
		public ICamera2D Camera { get; }
	}
}
