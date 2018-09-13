using Atlas.ECS.Objects;

namespace Atlas.Core.Messages
{
	class SleepMessage : PropertyMessage<ISleepObject, int>, ISleepMessage
	{
		public SleepMessage(ISleepObject messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
