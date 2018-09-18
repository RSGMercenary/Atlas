using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IUpdateStateMessage<out TMessenger> : IPropertyMessage<TMessenger, TimeStep>
		where TMessenger : IUpdateState
	{
	}
}
