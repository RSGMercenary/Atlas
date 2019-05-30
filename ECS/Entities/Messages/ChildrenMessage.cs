using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ChildrenMessage : Message<IEntity>, IChildrenMessage
	{
		public ChildrenMessage()
		{

		}
	}
}