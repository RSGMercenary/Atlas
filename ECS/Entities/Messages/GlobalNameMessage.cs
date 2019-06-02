using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class GlobalNameMessage : PropertyMessage<IEntity, string>, IGlobalNameMessage
	{
		public GlobalNameMessage(string current, string previous) : base(current, previous)
		{
		}
	}
}