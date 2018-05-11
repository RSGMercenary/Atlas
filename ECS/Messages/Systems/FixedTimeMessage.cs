using Atlas.ECS.Systems;

namespace Atlas.Framework.Messages
{
	class FixedTimeMessage : PropertyMessage<IReadOnlySystem, double>, IFixedTimeMessage
	{
		public FixedTimeMessage(double current, double previous) : base(current, previous)
		{
		}
	}
}
