using Atlas.Core.Messages;

namespace Atlas.ECS.Systems.Messages
{
	class PriorityMessage : PropertyMessage<ISystem, int>, IPriorityMessage
	{
		public PriorityMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}