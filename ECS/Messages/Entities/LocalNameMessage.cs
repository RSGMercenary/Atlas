using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class LocalNameMessage : PropertyMessage<IEntity, string>, ILocalNameMessage
	{
		public LocalNameMessage(string current, string previous) : base(current, previous)
		{
		}
	}
}
