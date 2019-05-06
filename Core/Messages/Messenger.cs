using Atlas.Core.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Messages
{
	public abstract class Messenger : IMessenger
	{
		private readonly Dictionary<Type, SignalBase> messages = new Dictionary<Type, SignalBase>();

		public Messenger()
		{

		}

		~Messenger()
		{
			Destroying();
		}

		public virtual void Dispose()
		{
			Disposing();
		}

		protected virtual void Destroying() { }

		/// <summary>
		/// Called when this instance is being disposed. Should not be called manually.
		/// </summary>
		protected virtual void Disposing()
		{
			foreach(var message in new List<Type>(messages.Keys))
			{
				var signal = messages[message];
				messages.Remove(message);
				signal.Dispose();
			}
		}

		public virtual void Message<TMessage>(TMessage message)
			where TMessage : IMessage
		{
			message.CurrentMessenger = this;
			//Pass around message internally...
			Messaging(message);
			//...before dispatching externally.
			var type = typeof(TMessage);
			if(messages.ContainsKey(type))
				(messages[type] as MessageSignal<TMessage>).Dispatch(message);
		}

		protected virtual void Messaging(IMessage message) { }

		public void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage
		{
			AddListener(listener, 0, MessageFlow.Self);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage
		{
			AddListener(listener, priority, MessageFlow.Self);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, MessageFlow flow)
			where TMessage : IMessage
		{
			AddListener(listener, 0, flow);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority, MessageFlow flow)
			where TMessage : IMessage
		{
			var type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				messages.Add(type, new MessageSignal<TMessage>());
			(messages[type] as MessageSignal<TMessage>).Add(listener, priority, flow);
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