using Atlas.ECS.Families;
using Atlas.Framework.Components.Render;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Render
{
	public class Camera2DMember : AtlasFamilyMember
	{
		public ICamera2D Camera { get; set; }
		public ITransform2D Transform { get; set; }
	}
}
