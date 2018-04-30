using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class ChildrenMessage : Message<IEntity>, IChildrenMessage
	{
		public ChildrenMessage()
		{

		}
	}
}