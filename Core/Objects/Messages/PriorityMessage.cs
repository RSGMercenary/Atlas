using Atlas.Core.Messages;
using Atlas.Core.Objects;

namespace Atlas.ECS.Systems.Messages
{
	class PriorityMessage<T> : PropertyMessage<T, int>, IPriorityMessage<T>
		where T : IPriority, IMessenger
	{
		public PriorityMessage(int current, int previous) : base(current, previous) { }
	}
}