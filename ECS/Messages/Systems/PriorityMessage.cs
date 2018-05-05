using Atlas.ECS.Systems;

namespace Atlas.Framework.Messages
{
	class PriorityMessage : PropertyMessage<IReadOnlySystem, int>, IPriorityMessage
	{
		public PriorityMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
