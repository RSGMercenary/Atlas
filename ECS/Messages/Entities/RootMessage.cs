using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class RootMessage : PropertyMessage<IEntity, IEntity>, IRootMessage
	{
		public RootMessage(IEntity messenger, IEntity current, IEntity previous) : base(messenger, current, previous)
		{
		}
	}
}
