using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class RootMessage : PropertyMessage<IEntity, IEntity>, IRootMessage
	{
		public RootMessage(IEntity current, IEntity previous) : base(current, previous)
		{
		}
	}
}
