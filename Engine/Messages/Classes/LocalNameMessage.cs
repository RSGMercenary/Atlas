using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class LocalNameMessage : PropertyMessage<IEntity, string>, ILocalNameMessage
	{
		public LocalNameMessage(string current, string previous) : base(current, previous)
		{
		}
	}
}
