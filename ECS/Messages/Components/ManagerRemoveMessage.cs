using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ManagerRemoveMessage : KeyValueMessage<IComponent, int, IEntity>, IManagerRemoveMessage
	{
		public ManagerRemoveMessage(IComponent messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
