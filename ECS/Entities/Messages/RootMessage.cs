using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class RootMessage : PropertyMessage<IEntity, IEntity>, IRootMessage
	{
		public RootMessage(IEntity current, IEntity previous) : base(current, previous)
		{
		}
	}
}