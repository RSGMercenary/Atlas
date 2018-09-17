using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IAutoDestroyMessage<out TMessenger> : IPropertyMessage<TMessenger, bool>
		where TMessenger : IAutoDestroyObject
	{
	}
}
