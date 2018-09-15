using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	class RootMessage : PropertyMessage<IEntity, IEntity>, IRootMessage
	{
		public RootMessage(IEntity messenger, IEntity current, IEntity previous) : base(messenger, current, previous)
		{
		}
	}
}
