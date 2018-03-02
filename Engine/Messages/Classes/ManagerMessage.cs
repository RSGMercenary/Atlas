using Atlas.Engine.Components;

namespace Atlas.Engine.Messages
{
	public class ManagerMessage : Message<IComponent>, IManagerMessage
	{
		public ManagerMessage()
		{

		}
	}
}