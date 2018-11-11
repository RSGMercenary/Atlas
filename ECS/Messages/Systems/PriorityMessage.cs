using Atlas.Core.Messages;
using Atlas.ECS.Systems;

namespace Atlas.ECS.Messages
{
	class PriorityMessage : PropertyMessage<ISystem, int>, IPriorityMessage
	{
		public PriorityMessage(ISystem messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
