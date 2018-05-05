using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class GlobalNameMessage : PropertyMessage<IEntity, string>, IGlobalNameMessage
	{
		public GlobalNameMessage(string current, string previous) : base(current, previous)
		{
		}
	}
}
