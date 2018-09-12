using Atlas.Framework.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Framework.Messages
{
	public abstract class MessageDispatcher : IMessageDispatcher
	{
		private Dictionary<Type, SignalBase> messages = new Dictionary<Type, SignalBase>();

		public virtual void Message<TMessage>(TMessage message)
			where TMessage : IMessage
		{
			Message(message, false);
		}

		protected void Message<TMessage>(TMessage message, bool hierarchy)
			where TMessage : IMessage
		{
			((IMessageBase)message).Messenger = this;
			((IMessageBase)message).CurrentMessenger = this;
			//Pass around message internally...
			Messaging(message);
			//...before dispatching externally.
			var type = typeof(TMessage);
			if(messages.ContainsKey(type))
				((Signal<TMessage>)messages[type]).Dispatch(message);
			if(!hierarchy)
			{
				//PoolManager.Push(message);
			}
		}

		protected virtual void Messaging(IMessage message)
		{

		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority = 0)
			where TMessage : IMessage
		{
			var type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				messages.Add(type, new Signal<TMessage>());
			var signal = (Signal<TMessage>)messages[type];
			signal.Add(listener, priority);
		}

		public void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage
		{
			var type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				return;
			var signal = messages[type];
			signal.Remove(listener);
			if(signal.Slots.Count > 0)
				return;
			messages.Remove(type);
			signal.Dispose();
		}
	}
}
