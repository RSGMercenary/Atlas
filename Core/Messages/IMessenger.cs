using System;

namespace Atlas.Core.Messages
{
	public interface IMessenger : IDisposable
	{
		void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage;

		void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage;

		void AddListener<TMessage>(Action<TMessage> listener, Func<TMessage, bool> validator)
			where TMessage : IMessage;

		void AddListener<TMessage>(Action<TMessage> listener, int priority, Func<TMessage, bool> validator)
			where TMessage : IMessage;

		void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage;

		void Message<TMessage>(TMessage message)
			where TMessage : IMessage;
	}
}
