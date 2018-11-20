using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class SleepMessage : PropertyMessage<ISleep, int>, ISleepMessage
	{
		public SleepMessage(ISleep messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
