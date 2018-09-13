using System;

namespace Atlas.Framework.Messages
{
	public interface IMessageDispatcher
	{
		void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage;

		void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage;

		void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage;

		void Message<TMessage>(TMessage message)
			where TMessage : IMessage;
	}
}
