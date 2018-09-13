using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	class ChildrenMessage : Message<IEntity>, IChildrenMessage
	{
		public ChildrenMessage(IEntity messenger) : base(messenger)
		{

		}
	}
}