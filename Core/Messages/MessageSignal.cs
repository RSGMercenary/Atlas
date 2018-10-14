using Atlas.Core.Signals;
using System;

namespace Atlas.Core.Messages
{
	public class MessageSignal<T> : Signal<T>
		where T : IMessage
	{
		protected override Slot<T> CreateGenericSlot()
		{
			return new MessageSlot<T>();
		}

		public ISlot<T> Add(Action<T> listener, int priority, Func<T, bool> validator)
		{
			var slot = (MessageSlot<T>)Add(listener, priority);
			slot.Validator = validator;
			return slot;
		}
	}
}
