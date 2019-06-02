using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class LocalNameMessage : PropertyMessage<IEntity, string>, ILocalNameMessage
	{
		public LocalNameMessage(string current, string previous) : base(current, previous)
		{
		}
	}
}