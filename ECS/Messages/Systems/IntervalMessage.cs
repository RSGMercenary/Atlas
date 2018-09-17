using Atlas.Core.Messages;
using Atlas.ECS.Systems;

namespace Atlas.ECS.Messages
{
	class IntervalMessage : PropertyMessage<IReadOnlySystem, double>, IIntervalMessage
	{
		public IntervalMessage(IReadOnlySystem messenger, double current, double previous) : base(messenger, current, previous)
		{
		}
	}
}
