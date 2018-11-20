using Atlas.Core.Messages;

namespace Atlas.ECS.Systems.Messages
{
	class PriorityMessage : PropertyMessage<ISystem, int>, IPriorityMessage
	{
		public PriorityMessage(ISystem messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
