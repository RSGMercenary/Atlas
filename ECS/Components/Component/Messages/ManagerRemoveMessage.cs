using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	class ManagerRemoveMessage<T> : KeyValueMessage<T, int, IEntity>, IManagerRemoveMessage<T>
		where T : class, IComponent
	{
		public ManagerRemoveMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
