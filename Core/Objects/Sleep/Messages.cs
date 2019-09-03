using Atlas.Core.Messages;

namespace Atlas.Core.Objects.Sleep
{
	public interface ISleepMessage<out T> : IPropertyMessage<T, int> where T : ISleep, IMessenger { }

	class SleepMessage<T> : PropertyMessage<T, int>, ISleepMessage<T>
		where T : ISleep, IMessenger
	{
		public SleepMessage(int current, int previous) : base(current, previous) { }
	}
}