using Atlas.Core.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Messages
{
	public abstract class Messenger<TMessenger> : IMessenger<TMessenger>
		where TMessenger : IMessenger
	{
		public static implicit operator bool(Messenger<TMessenger> instance)
		{
			return instance != null;
		}

		private readonly Dictionary<Type, SignalBase> messages = new Dictionary<Type, SignalBase>();

		protected virtual void Messaging(IMessage message)
		{

		}

		public void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<TMessenger>
		{
			AddListenerSlot(listener, 0);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<TMessenger>
		{
			AddListenerSlot(listener, priority);
		}

		protected ISlotBase AddListenerSlot<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage
		{
			var type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				messages.Add(type, CreateSignal<TMessage>());
			return messages[type].Add(listener, priority);
		}

		public void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<TMessenger>
		{
			(this as IMessenger).RemoveListener(listener);
		}

		protected virtual Signal<TMessage> CreateSignal<TMessage>()
			where TMessage : IMessage
		{
			return new Signal<TMessage>();
		}

		#region IMessenger

		public virtual void Dispatch<TMessage>(TMessage message)
			where TMessage : IMessage
		{
			message.CurrentMessenger = this;
			//Pass around message internally...
			Messaging(message);
			//...before dispatching externally.
			var type = typeof(TMessage);
			if(messages.ContainsKey(type))
				((Signal<TMessage>)messages[type]).Dispatch(message);
		}

		void IMessenger.AddListener<TMessage>(Action<TMessage> listener)
		{
			AddListenerSlot(listener, 0);
		}

		void IMessenger.AddListener<TMessage>(Action<TMessage> listener, int priority)
		{
			AddListenerSlot(listener, priority);
		}

		void IMessenger.RemoveListener<TMessage>(Action<TMessage> listener)
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

		#endregion
	}
}