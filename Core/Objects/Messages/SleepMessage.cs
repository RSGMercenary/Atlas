using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class SleepMessage<T> : PropertyMessage<T, int>, ISleepMessage<T>
		where T : ISleep, IMessenger
	{
		public SleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}