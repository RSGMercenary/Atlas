using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	class EntityAddMessage : ValueMessage<IEngine, IEntity>, IEntityAddMessage
	{
		public EntityAddMessage(IEngine messenger, IEntity value) : base(messenger, value)
		{
		}
	}
}
