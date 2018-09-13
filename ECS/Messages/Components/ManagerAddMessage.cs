using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ManagerAddMessage : KeyValueMessage<IComponent, int, IEntity>, IManagerAddMessage
	{
		public ManagerAddMessage(IComponent messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
