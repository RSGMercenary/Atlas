using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ChildRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IChildRemoveMessage
	{
		public ChildRemoveMessage(IEntity messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
