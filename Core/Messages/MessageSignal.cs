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

		public ISlot<TMessage> Add(Action<TMessage> listener, int priority, MessageFlow flow)
		{
			var slot = (MessageSlot<TMessage>)Add(listener, priority);
			slot.Flow = flow;
			return slot;
		}
	}
}
