using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ParentIndexMessage : PropertyMessage<IEntity, int>, IParentIndexMessage
	{
		public ParentIndexMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}