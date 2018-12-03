using Atlas.ECS.Families;
using Atlas.Framework.Components.Transform;

namespace Atlas.Framework.Families.Transform
{
	public class Cursor2DMember : AtlasFamilyMember
	{
		public ITransform2D Transform;
		public ICursor2D Cursor;
	}
}
