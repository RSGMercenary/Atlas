using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IUpdateStateMessage<T> : IPropertyMessage<T, TimeStep>
		where T : IUpdateState
	{
	}
}