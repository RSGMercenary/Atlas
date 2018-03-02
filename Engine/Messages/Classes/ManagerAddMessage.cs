using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class ManagerAddMessage : KeyValueMessage<IEntity, int, IEntity>, IManagerAddMessage
	{
		public ManagerAddMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
