using Atlas.Core.Messages;
using Atlas.Core.Signals;

namespace Atlas.Core.Collections.Hierarchy
{
	public class HierarchySignal<TMessage, T> : Signal<TMessage>
		where TMessage : IMessage<T>
		where T : class, IMessenger, IHierarchy<T>
	{
		protected override Slot<TMessage> CreateGenericSlot()
		{
			return new HierarchySlot<TMessage, T>();
		}
	}
}