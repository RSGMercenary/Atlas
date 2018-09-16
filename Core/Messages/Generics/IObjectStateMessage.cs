using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	public interface IObjectStateMessage<TMessenger> : IPropertyMessage<TMessenger, ObjectState>
		where TMessenger : IObject
	{
	}
}
