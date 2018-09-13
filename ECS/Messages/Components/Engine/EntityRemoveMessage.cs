using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	class EntityRemoveMessage : ValueMessage<IEngine, IEntity>, IEntityRemoveMessage
	{
		public EntityRemoveMessage(IEngine messenger, IEntity value) : base(messenger, value)
		{
		}
	}
}
