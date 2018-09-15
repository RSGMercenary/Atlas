using Atlas.Core.Messages;
using Atlas.ECS.Systems;

namespace Atlas.ECS.Messages
{
	class PriorityMessage : PropertyMessage<IReadOnlySystem, int>, IPriorityMessage
	{
		public PriorityMessage(IReadOnlySystem messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
