using Atlas.ECS.Components;

namespace Atlas.Framework.Messages
{
	class ManagerMessage : Message<IComponent>, IManagerMessage
	{
		public ManagerMessage(IComponent messenger) : base(messenger)
		{

		}
	}
}