using Atlas.Engine.Systems;

namespace Atlas.Engine.Messages
{
	interface IPriorityMessage : IPropertyMessage<ISystem, int>
	{
	}
}
