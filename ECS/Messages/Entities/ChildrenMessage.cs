using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ChildrenMessage : Message<IEntity>, IChildrenMessage
	{
		public ChildrenMessage(IEntity messenger) : base(messenger)
		{

		}
	}
}