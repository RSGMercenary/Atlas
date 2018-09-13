using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class LocalNameMessage : PropertyMessage<IEntity, string>, ILocalNameMessage
	{
		public LocalNameMessage(IEntity messenger, string current, string previous) : base(messenger, current, previous)
		{
		}
	}
}
