using Atlas.ECS.Families;
using Atlas.Framework.Components.Render;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Render
{
	public class Render2DMember : AtlasFamilyMember
	{
		private ITransform2D transform;
		public ITransform2D Transform { get { return transform; } }

		private IRender2D render;
		public IRender2D Render { get { return render; } }
	}
}
