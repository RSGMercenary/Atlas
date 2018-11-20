using Atlas.Core.Signals;
using System;

namespace Atlas.Core.Messages
{
	public class MessageSlot<TMessage> : Slot<TMessage>
		where TMessage : IMessage
	{
		public Func<TMessage, bool> Validator { get; set; } = Messenger.Self;

		public override bool Dispatch(TMessage item1)
		{
			if(Validator != null && !Validator(item1))
				return false;
			return base.Dispatch(item1);
		}
	}
}
