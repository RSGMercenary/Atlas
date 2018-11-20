using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	class EntityRemoveMessage : ValueMessage<IEngine, IEntity>, IEntityRemoveMessage
	{
		public EntityRemoveMessage(IEngine messenger, IEntity value) : base(messenger, value)
		{
		}
	}
}
