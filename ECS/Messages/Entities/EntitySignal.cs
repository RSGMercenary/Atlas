using Atlas.Framework.Messages.Signals;
using Atlas.Framework.Signals;

namespace Atlas.Framework.Messages
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
