using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ManagerRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IManagerRemoveMessage
	{
		public ManagerRemoveMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
