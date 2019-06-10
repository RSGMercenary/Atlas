using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IAutoDisposeMessage<T> : IPropertyMessage<T, bool>
		where T : IAutoDispose
	{
	}
}