using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface ISleepMessage<T> : IPropertyMessage<T, int>
		where T : ISleep
	{
	}
}
