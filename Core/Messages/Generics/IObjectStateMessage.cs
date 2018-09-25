using Atlas.Core.Objects;
using Atlas.ECS.Objects;

namespace Atlas.Core.Messages
{
	public interface IObjectStateMessage<out TMessenger> : IPropertyMessage<TMessenger, ObjectState>
		where TMessenger : IObject
	{
	}
}
