using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	class ManagerAddMessage<T> : KeyValueMessage<T, int, IEntity>, IManagerAddMessage<T>
		where T : IComponent
	{
		public ManagerAddMessage(int key, IEntity value) : base(key, value) { }
	}
}