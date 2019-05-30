using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Messages
{
	class ManagerMessage<T> : Message<T>, IManagerMessage<T>
		where T : class, IComponent
	{
		public ManagerMessage()
		{

		}
	}
}