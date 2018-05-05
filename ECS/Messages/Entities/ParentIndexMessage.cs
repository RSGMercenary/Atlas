using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ParentIndexMessage : PropertyMessage<IEntity, int>, IParentIndexMessage
	{
		public ParentIndexMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
