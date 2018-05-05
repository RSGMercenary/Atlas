using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class EntityAddMessage : ValueMessage<IEngine, IEntity>, IEntityAddMessage
	{
		public EntityAddMessage(IEntity value) : base(value)
		{
		}
	}
}
