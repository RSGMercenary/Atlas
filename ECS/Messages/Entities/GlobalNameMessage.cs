using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class GlobalNameMessage : PropertyMessage<IEntity, string>, IGlobalNameMessage
	{
		public GlobalNameMessage(IEntity messenger, string current, string previous) : base(messenger, current, previous)
		{
		}
	}
}
