using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ParentMessage : PropertyMessage<IEntity, IEntity>, IParentMessage
	{
		public ParentMessage(IEntity messenger, IEntity current, IEntity previous) : base(messenger, current, previous)
		{
		}
	}
}
