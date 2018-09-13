using Atlas.ECS.Components;

namespace Atlas.Core.Messages
{
	class ManagerMessage : Message<IComponent>, IManagerMessage
	{
		public ManagerMessage(IComponent messenger) : base(messenger)
		{

		}
	}
}