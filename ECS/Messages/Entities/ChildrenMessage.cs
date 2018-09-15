using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	class ChildrenMessage : Message<IEntity>, IChildrenMessage
	{
		public ChildrenMessage(IEntity messenger) : base(messenger)
		{

		}
	}
}