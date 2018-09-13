using Atlas.ECS.Objects;

namespace Atlas.Core.Messages
{
	public interface IAutoDestroyMessage : IPropertyMessage<IAutoDestroyObject, bool>
	{
	}
}
