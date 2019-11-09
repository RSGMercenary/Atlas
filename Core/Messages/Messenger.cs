using Atlas.Core.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Messages
{
	public abstract class Messenger<T> : IMessenger<T>
		where T : class, IMessenger
	{
		//protected T Target { get; }
		//private Action<IMessage<T>> Callout { get; }
		private readonly Dictionary<Type, SignalBase> messages = new Dictionary<Type, SignalBase>();

		/*
		public Messenger(T target = null, Action<IMessage<T>> callout = null)
		{
			Target = target ?? this as T;
			Callout = callout;
		}
		*/

		public virtual void Dispose()
		{
			Disposing();
		}

		protected virtual void Disposing()
		{
			RemoveListeners();
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

		protected virtual void Messaging(IMessage<T> message)
		{
			//Callout?.Invoke(message);
		}

		public void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T>
		{
			AddListenerSlot(listener, 0);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<T>
		{
			AddListenerSlot(listener, priority);
		}

		protected ISlot<TMessage> AddListenerSlot<TMessage>(Action<TMessage> listener, int priority)
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

		public bool RemoveListeners()
		{
			if(messages.Count <= 0)
				return false;
			foreach(var message in new List<Type>(messages.Keys))
			{
				var signal = messages[message];
				messages.Remove(message);
				signal.Dispose();
			}
			return true;
		}

		protected virtual Signal<TMessage> CreateSignal<TMessage>()
			where TMessage : IMessage<T>
		{
			return new Signal<TMessage>();
		}
	}
}