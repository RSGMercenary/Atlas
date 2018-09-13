using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ParentMessage : PropertyMessage<IEntity, IEntity>, IParentMessage
	{
		public ParentMessage(IEntity messenger, IEntity current, IEntity previous) : base(messenger, current, previous)
		{
		}
	}
}
