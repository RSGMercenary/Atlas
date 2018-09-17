using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IObjectStateMessage<out TMessenger> : IPropertyMessage<TMessenger, ObjectState>
		where TMessenger : IObject
	{
	}
}
