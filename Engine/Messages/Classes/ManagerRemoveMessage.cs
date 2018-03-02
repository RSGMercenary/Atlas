using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class ManagerRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IManagerRemoveMessage
	{
		public ManagerRemoveMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
