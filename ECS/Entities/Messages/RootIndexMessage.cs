using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class RootIndexMessage : PropertyMessage<IEntity, int>, IRootIndexMessage
	{
		public RootIndexMessage(IEntity messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
