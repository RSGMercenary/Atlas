using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class SleepMessage<T> : PropertyMessage<T, int>, ISleepMessage<T>
		where T : ISleep
	{
		public SleepMessage(T messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
