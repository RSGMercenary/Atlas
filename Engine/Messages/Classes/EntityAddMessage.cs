using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class EntityAddMessage : ValueMessage<IEntity, IEntity>, IEntityAddMessage
	{
		public EntityAddMessage(IEntity value) : base(value)
		{
		}
	}
}
