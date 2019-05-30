using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class SleepMessage<T> : PropertyMessage<T, int>, ISleepMessage<T>
		where T : class, ISleep
	{
		public SleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
