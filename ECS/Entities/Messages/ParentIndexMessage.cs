using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ParentIndexMessage : PropertyMessage<IEntity, int>, IParentIndexMessage
	{
		public ParentIndexMessage(IEntity messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
