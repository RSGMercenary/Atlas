using Atlas.Engine.Systems;

namespace Atlas.Engine.Messages
{
	class PriorityMessage : PropertyMessage<ISystem, int>, IPriorityMessage
	{
		public PriorityMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
