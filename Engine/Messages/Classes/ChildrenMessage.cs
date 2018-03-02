using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	public class ChildrenMessage : Message<IEntity>, IChildrenMessage
	{
		public ChildrenMessage()
		{

		}
	}
}