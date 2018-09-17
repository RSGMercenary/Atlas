using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface ISleepMessage<out TMessenger> : IPropertyMessage<TMessenger, int>
		where TMessenger : ISleepObject
	{
	}
}
