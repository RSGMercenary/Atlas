using Atlas.ECS.Entities;

namespace Atlas.ECS.Families
{
	public class AtlasFamilyMember : IFamilyMember
	{
		private IEntity entity;

		public IEntity Entity { get { return entity; } }
	}
}
