using Atlas.Core.Signals;

namespace Atlas.Core.Messages
{
	public class MessageSignal<T> : Signal<T>
		where T : IMessage
	{
		protected override Slot<T> CreateGenericSlot()
		{
			var slot = base.CreateGenericSlot();
			slot.Validator = Messenger.Self;
			return slot;
		}
	}
}
