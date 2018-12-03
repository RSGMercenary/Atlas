using Atlas.ECS.Families;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Transform
{
	public class Cursor2DMember : AtlasFamilyMember
	{
		private ITransform2D transform;
		public ITransform2D Transform { get { return transform; } }

		private ICursor2D cursor;
		public ICursor2D Cursor { get { return cursor; } }
	}
}
