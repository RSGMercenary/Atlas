using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IUpdateStateMessage<out T> : IPropertyMessage<T, TimeStep>
		where T : IUpdateState, IMessenger
	{
	}
}