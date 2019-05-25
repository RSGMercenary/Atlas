using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Messages
{
	class ManagerMessage<T> : Message<T>, IManagerMessage<T>
		where T : IComponent
	{
		public ManagerMessage(T messenger) : base(messenger)
		{

		}
	}
}