using Atlas.Core.Messages;
using Atlas.Signals;

namespace Atlas.Core.Collections.Hierarchy;

public class HierarchySignal<TMessage, T> : Signal<TMessage>
	where TMessage : IMessage<T>
	where T : class, IHierarchyMessenger<T>
{
	protected override SlotBase CreateSlot()
	{
		return new HierarchySlot<TMessage, T>();
	}
}