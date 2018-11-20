using Atlas.Core.Signals;
using System;

namespace Atlas.Core.Messages
{
	public class MessageSignal<TMessage> : Signal<TMessage>
		where TMessage : IMessage
	{
		protected override Slot<TMessage> CreateGenericSlot()
		{
			return new MessageSlot<TMessage>();
		}

		public ISlot<TMessage> Add(Action<TMessage> listener, int priority, Func<TMessage, bool> validator)
		{
			var slot = (MessageSlot<TMessage>)Add(listener, priority);
			slot.Validator = validator;
			return slot;
		}
	}
}
