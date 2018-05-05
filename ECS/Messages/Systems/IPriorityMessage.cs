using Atlas.ECS.Systems;

namespace Atlas.Framework.Messages
{
	public interface IPriorityMessage : IPropertyMessage<IReadOnlySystem, int>
	{
	}
}
