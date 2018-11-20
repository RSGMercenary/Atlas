using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Messages
{
	class ManagerMessage : Message<IComponent>, IManagerMessage
	{
		public ManagerMessage(IComponent messenger) : base(messenger)
		{

		}
	}
}