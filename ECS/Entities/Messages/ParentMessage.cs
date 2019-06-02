using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ParentMessage : PropertyMessage<IEntity, IEntity>, IParentMessage
	{
		public ParentMessage(IEntity current, IEntity previous) : base(current, previous)
		{
		}
	}
}