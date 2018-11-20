using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	class ManagerRemoveMessage : KeyValueMessage<IComponent, int, IEntity>, IManagerRemoveMessage
	{
		public ManagerRemoveMessage(IComponent messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
