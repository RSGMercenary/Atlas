using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class EntityRemoveMessage : ValueMessage<IEngine, IEntity>, IEntityRemoveMessage
	{
		public EntityRemoveMessage(IEntity value) : base(value)
		{
		}
	}
}
