using Atlas.Core.Signals;

namespace Atlas.Core.Messages
{
	public class HierarchySignal<TMessage, T> : Signal<TMessage>
		where TMessage : IMessage<T>
		where T : class, IHierarchyMessenger<T>
	{
		protected override Slot<TMessage> CreateGenericSlot()
		{
			return new HierarchySlot<TMessage, T>();
		}
	}
}