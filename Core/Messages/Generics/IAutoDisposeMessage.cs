using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IAutoDisposeMessage<out TMessenger> : IPropertyMessage<TMessenger, bool>
		where TMessenger : IAutoDispose
	{
	}
}
