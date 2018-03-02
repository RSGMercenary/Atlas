using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class ParentMessage : PropertyMessage<IEntity, IEntity>, IParentMessage
	{
		public ParentMessage(IEntity current, IEntity previous) : base(current, previous)
		{
		}
	}
}
