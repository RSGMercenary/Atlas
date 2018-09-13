using Atlas.ECS.Objects;

namespace Atlas.Framework.Messages
{
	public interface IAutoDestroyMessage : IPropertyMessage<IAutoDestroyObject, bool>
	{
	}
}
