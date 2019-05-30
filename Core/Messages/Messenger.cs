using Atlas.Core.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Messages
{
	public abstract class Messenger<T> : IMessenger<T>
		where T : class, IMessenger
	{
		private readonly Dictionary<Type, SignalBase> messages = new Dictionary<Type, SignalBase>();

		public Messenger()
		{

		}

		public virtual void Dispose()
		{
			Disposing();
		}

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
			where TMessage : IMessage<T>
		{
			(message as IMessage).Messenger = this;
			(message as IMessage).CurrentMessenger = this;
			//Pass around message internally...
			Messaging(message);
			//...before dispatching externally.
			var type = typeof(TMessage);
			if(messages.ContainsKey(type))
				(messages[type] as Signal<TMessage>).Dispatch(message);
		}

		protected virtual void Messaging(IMessage<T> message) { }

		public void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T>
		{
			AddListenerSlot(listener);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<T>
		{
			AddListenerSlot(listener, priority);
		}

		protected ISlot<TMessage> AddListenerSlot<TMessage>(Action<TMessage> listener, int priority = 0)
			where TMessage : IMessage<T>
		{
			var type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				messages.Add(type, CreateSignal<TMessage>());
			return (messages[type] as Signal<TMessage>).Add(listener, priority);
		}

		public void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T>
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

		protected virtual Signal<TMessage> CreateSignal<TMessage>()
			where TMessage : IMessage<T>
		{
			return new Signal<TMessage>();
		}
	}
}