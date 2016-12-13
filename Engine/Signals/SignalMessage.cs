using Atlas.Testing.Messages;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Signals
{
	class SignalMessage<TMessage, TSender>:Signal<TMessage>, ISignalMessage<TMessage, TSender> where TMessage : IMessage<TSender>
	{
		private Stack<TMessage> messagesPooled = new Stack<TMessage>();
		private HashSet<TMessage> messagesManaged = new HashSet<TMessage>();
		private Queue<TMessage> messagesQueued;

		public SignalMessage() : this(false)
		{

		}

		public SignalMessage(bool isQueue = false)
		{
			if(isQueue)
				messagesQueued = new Queue<TMessage>();
		}

		public bool IsQueue
		{
			get
			{
				return messagesQueued != null;
			}
		}

		public bool Dispatch(string type, TSender sender)
		{
			if(!HasListeners)
				return false;
			TMessage message = CreateMessage();
			//message.Initialize(type, sender);
			return Dispatch(message);
		}

		override public bool Dispatch(TMessage message)
		{
			if(IsQueue && IsDispatching)
			{
				//We could be dispatching, but all the listeners have been removed.
				if(HasListeners)
				{
					messagesQueued.Enqueue(message);
					return true;
				}
				else
				{
					DisposeMessage(message);
					return false;
				}
			}
			bool success = base.Dispatch(message);
			DisposeMessage(message);
			if(IsQueue)
			{
				if(!HasListeners)
				{
					//No listeners. The queue is useless.
					while(messagesQueued.Count > 0)
					{
						DisposeMessage(messagesQueued.Dequeue());
					}
				}
				else if(messagesQueued.Count > 0)
				{
					Dispatch(messagesQueued.Dequeue());
				}
			}
			return success;
		}

		protected TMessage CreateMessage()
		{
			TMessage message = default(TMessage);
			if(messagesPooled.Count > 0)
			{
				message = messagesPooled.Pop();
			}
			else
			{
				message = Activator.CreateInstance<TMessage>();
			}
			messagesManaged.Add(message);
			return message;
		}

		protected void DisposeMessage(TMessage message)
		{
			message.Dispose();
			if(messagesManaged.Contains(message))
			{
				messagesManaged.Remove(message);
				if(!IsDisposed)
					messagesPooled.Push(message);
			}
		}
	}
}
