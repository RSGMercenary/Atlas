using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface ISleepMessage<out T> : IPropertyMessage<T, int>
		where T : ISleep, IMessenger
	{
	}
}