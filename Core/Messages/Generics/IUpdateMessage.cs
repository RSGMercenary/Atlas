using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IUpdateMessage<TMessenger> : IPropertyMessage<TMessenger, bool>
		where TMessenger : IUpdateObject
	{
	}
}
