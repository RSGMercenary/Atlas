using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IAutoDisposeMessage<out T> : IPropertyMessage<T, bool>
		where T : IAutoDispose, IMessenger
	{
	}
}