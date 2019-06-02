using Atlas.Core.Messages;

namespace Atlas.ECS.Systems.Messages
{
	public interface IPriorityMessage : IPropertyMessage<ISystem, int>
	{
	}
}