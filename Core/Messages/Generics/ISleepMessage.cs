using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface ISleepMessage<TMessenger> : IPropertyMessage<TMessenger, int>
		where TMessenger : ISleepObject
	{
	}
}
