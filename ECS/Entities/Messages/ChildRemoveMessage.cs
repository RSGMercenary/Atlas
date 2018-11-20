using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ChildRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IChildRemoveMessage
	{
		public ChildRemoveMessage(IEntity messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
