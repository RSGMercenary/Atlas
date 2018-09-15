using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	class ChildRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IChildRemoveMessage
	{
		public ChildRemoveMessage(IEntity messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
