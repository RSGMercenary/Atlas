using Atlas.Messages;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Signals
{
	class SignalMessage<TMessage, TSender>:Signal<TMessage>, ISignalMessage<TMessage, TSender> where TMessage : Message<TSender>
	{
		private Stack<TMessage> messagesPooled = new Stack<TMessage>();

		public SignalMessage()
		{

		}

		public void Dispatch(TSender sender)
		{
			TMessage message;
			if(messagesPooled.Count > 0)
			{
				message = messagesPooled.Pop();
			}
			else
			{
				message = Activator.CreateInstance<TMessage>();
			}
			message.Initialize("", sender);
			Dispatch(message);
			if(!IsDisposed)
				messagesPooled.Push(message);
		}

		public void Dispatch(TMessage message)
		{
			if(DispatchStart())
			{
				foreach(Slot<TMessage> slot in Slots)
				{
					try
					{
						slot.Listener.Invoke(message);
					}
					catch
					{
						//We remove the Slot so the Error doesn't inevitably happen again.
						Remove(slot.Listener);
					}
				}
				message.Dispose();
				DispatchStop();
			}
		}
	}
}
