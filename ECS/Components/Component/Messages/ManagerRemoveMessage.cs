using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	class ManagerRemoveMessage<T> : KeyValueMessage<T, int, IEntity>, IManagerRemoveMessage<T>
		where T : IComponent
	{
		public ManagerRemoveMessage(T messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
