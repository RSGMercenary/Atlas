using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	class RootMessage : PropertyMessage<IEntity, IEntity>, IRootMessage
	{
		public RootMessage(IEntity messenger, IEntity current, IEntity previous) : base(messenger, current, previous)
		{
		}
	}
}
