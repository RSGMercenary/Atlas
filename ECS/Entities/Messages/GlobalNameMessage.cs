using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class GlobalNameMessage : PropertyMessage<IEntity, string>, IGlobalNameMessage
	{
		public GlobalNameMessage(IEntity messenger, string current, string previous) : base(messenger, current, previous)
		{
		}
	}
}
