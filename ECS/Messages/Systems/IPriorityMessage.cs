using Atlas.ECS.Systems;

namespace Atlas.Core.Messages
{
	public interface IPriorityMessage : IPropertyMessage<IReadOnlySystem, int>
	{
	}
}
