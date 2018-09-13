using Atlas.ECS.Systems;

namespace Atlas.Core.Messages
{
	class FixedTimeMessage : PropertyMessage<IReadOnlySystem, double>, IFixedTimeMessage
	{
		public FixedTimeMessage(IReadOnlySystem messenger, double current, double previous) : base(messenger, current, previous)
		{
		}
	}
}
