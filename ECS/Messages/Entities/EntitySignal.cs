using Atlas.Core.Messages.Signals;
using Atlas.Core.Signals;

namespace Atlas.Core.Messages
{
	public class EntitySignal<T> : Signal<T>
		where T : IMessage
	{
		protected override SlotBase CreateSlot()
		{
			return new EntitySlot<T>();
		}
	}
}
