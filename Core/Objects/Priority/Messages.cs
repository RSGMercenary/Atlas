using Atlas.Core.Messages;

namespace Atlas.Core.Objects.Priority
{
	public interface IPriorityMessage<out T> : IPropertyMessage<T, int> where T : IPriority, IMessenger { }

	class PriorityMessage<T> : PropertyMessage<T, int>, IPriorityMessage<T>
		where T : IPriority, IMessenger
	{
		public PriorityMessage(int current, int previous) : base(current, previous) { }
	}
}