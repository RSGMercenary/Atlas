using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ChildRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IChildRemoveMessage
	{
		public ChildRemoveMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}