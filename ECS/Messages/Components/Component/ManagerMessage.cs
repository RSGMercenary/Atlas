using Atlas.Core.Messages;
using Atlas.ECS.Components;

namespace Atlas.ECS.Messages
{
	class ManagerMessage : Message<IComponent>, IManagerMessage
	{
		public ManagerMessage(IComponent messenger) : base(messenger)
		{

		}
	}
}