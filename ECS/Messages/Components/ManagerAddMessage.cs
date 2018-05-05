using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ManagerAddMessage : KeyValueMessage<IEntity, int, IEntity>, IManagerAddMessage
	{
		public ManagerAddMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
