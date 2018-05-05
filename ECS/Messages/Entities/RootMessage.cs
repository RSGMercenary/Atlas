using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class RootMessage : PropertyMessage<IEntity, IEntity>, IRootMessage
	{
		public RootMessage(IEntity current, IEntity previous) : base(current, previous)
		{
		}
	}
}
