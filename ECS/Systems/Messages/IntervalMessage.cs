using Atlas.Core.Messages;

namespace Atlas.ECS.Systems.Messages
{
	class IntervalMessage : PropertyMessage<ISystem, double>, IIntervalMessage
	{
		public IntervalMessage(double current, double previous) : base(current, previous) { }
	}
}