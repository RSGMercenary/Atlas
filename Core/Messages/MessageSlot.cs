using Atlas.Core.Signals;
using System;

namespace Atlas.Core.Messages
{
	public class MessageSlot<T> : Slot<T>
		where T : IMessage
	{
		public Func<T, bool> Validator { get; set; } = Messenger.Self;

		public override bool Dispatch(T item1)
		{
			if(Validator != null && !Validator(item1))
				return false;
			return base.Dispatch(item1);
		}
	}
}
