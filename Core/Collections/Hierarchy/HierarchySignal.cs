using Atlas.Core.Messages;
using Atlas.Core.Signals;

namespace Atlas.Core.Collections.Hierarchy
{
	public class HierarchySignal<T> : Signal<T>
		where T : IMessage
	{
		protected override SlotBase CreateSlot()
		{
			return new HierarchySlot<T>();
		}
	}
}
