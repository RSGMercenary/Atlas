using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IPriorityMessage<T> : IPropertyMessage<T, int>
		where T : IPriority, IMessenger
	{
	}
}