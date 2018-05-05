using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ParentMessage : PropertyMessage<IEntity, IEntity>, IParentMessage
	{
		public ParentMessage(IEntity current, IEntity previous) : base(current, previous)
		{
		}
	}
}
