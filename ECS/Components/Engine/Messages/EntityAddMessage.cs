using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	class EntityAddMessage : ValueMessage<IEngine, IEntity>, IEntityAddMessage
	{
		public EntityAddMessage(IEntity value) : base(value)
		{
		}
	}
}
