using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	class EntityRemoveMessage : ValueMessage<IEngine, IEntity>, IEntityRemoveMessage
	{
		public EntityRemoveMessage(IEntity value) : base(value)
		{
		}
	}
}
