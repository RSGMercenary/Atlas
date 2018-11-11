using Atlas.Core.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Messages
{
	public static class Messenger
	{
		public static bool Self<TMessage>(TMessage message)
			where TMessage : IMessage
		{
			return message.Messenger == message.CurrentMessenger;
		}

		public static bool All<TMessage>(TMessage message)
			where TMessage : IMessage
		{
			return true;
		}
	}

	public abstract class Messenger<TMessenger> : IMessenger<TMessenger>
		where TMessenger : IMessenger
	{
		private readonly Dictionary<Type, SignalBase> messages = new Dictionary<Type, SignalBase>();

		public Messenger()
		{
			Compose(true);
		}

		~Messenger()
		{
			Dispose(true);
		}

		public virtual void Compose()
		{
			Compose(false);
		}

		public virtual void Dispose()
		{
			Dispose(false);
		}

		private void Compose(bool constructor)
		{
			Composing(constructor);
			if(!constructor)
				GC.ReRegisterForFinalize(this);
		}

		private void Dispose(bool finalizer)
		{
			Disposing(finalizer);
			if(!finalizer)
				GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Called when this instance is being composed. Should not be called manually.
		/// </summary>
		protected virtual void Composing(bool constructor) { }

		/// <summary>
		/// Called when this instance is being disposed. Should not be called manually.
		/// </summary>
		protected virtual void Disposing(bool finalizer)
		{
			if(!finalizer)
			{
				foreach(var message in new List<Type>(messages.Keys))
				{
					var signal = messages[message];
					messages.Remove(message);
					signal.Dispose();
				}
			}
		}

		protected virtual void Messaging(IMessage message) { }

		public void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<TMessenger>
		{
			(this as IMessenger).AddListener(listener, 0, null);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<TMessenger>
		{
			(this as IMessenger).AddListener(listener, priority, null);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, Func<TMessage, bool> validator)
			where TMessage : IMessage<TMessenger>
		{
			(this as IMessenger).AddListener(listener, 0, validator);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority, Func<TMessage, bool> validator)
			where TMessage : IMessage<TMessenger>
		{
			(this as IMessenger).AddListener(listener, priority, validator);
		}

		public void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<TMessenger>
		{
			(this as IMessenger).RemoveListener(listener);
		}

		#region IMessenger

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

		void IMessenger.AddListener<TMessage>(Action<TMessage> listener)
		{
			(this as IMessenger).AddListener(listener, 0, null);
		}

		void IMessenger.AddListener<TMessage>(Action<TMessage> listener, int priority)
		{
			(this as IMessenger).AddListener(listener, priority, null);
		}

		void IMessenger.AddListener<TMessage>(Action<TMessage> listener, Func<TMessage, bool> validator)
		{
			(this as IMessenger).AddListener(listener, 0, validator);
		}

		void IMessenger.AddListener<TMessage>(Action<TMessage> listener, int priority, Func<TMessage, bool> validator)
		{
			var type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				messages.Add(type, new MessageSignal<TMessage>());
			(messages[type] as MessageSignal<TMessage>).Add(listener, priority, validator);
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