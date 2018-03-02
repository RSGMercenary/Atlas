using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class EntityRemoveMessage : ValueMessage<IEntity, IEntity>, IEntityRemoveMessage
	{
		public EntityRemoveMessage(IEntity value) : base(value)
		{
		}
	}
}
