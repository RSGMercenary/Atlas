using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IAutoDestroyMessage<TMessenger> : IPropertyMessage<TMessenger, bool>
		where TMessenger : IAutoDestroyObject
	{
	}
}
