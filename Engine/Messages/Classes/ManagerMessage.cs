using Atlas.Engine.Components;

namespace Atlas.Engine.Messages
{
	class ManagerMessage : Message<IComponent>, IManagerMessage
	{
		public ManagerMessage()
		{

		}
	}
}