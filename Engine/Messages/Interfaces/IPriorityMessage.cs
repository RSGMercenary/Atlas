using Atlas.Engine.Systems;

namespace Atlas.Engine.Messages
{
	public interface IPriorityMessage : IPropertyMessage<ISystem, int>
	{
	}
}
