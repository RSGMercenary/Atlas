using Atlas.Core.Messages;

namespace Atlas.ECS.Systems.Messages
{
	class IntervalMessage : PropertyMessage<ISystem, double>, IIntervalMessage
	{
		public IntervalMessage(ISystem messenger, double current, double previous) : base(messenger, current, previous)
		{
		}
	}
}
