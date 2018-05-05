using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ChildRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IChildRemoveMessage
	{
		public ChildRemoveMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
