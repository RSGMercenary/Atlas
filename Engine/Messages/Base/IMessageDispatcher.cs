using System;

namespace Atlas.Engine.Messages
{
	public interface IMessageDispatcher
	{
		void AddListener<TMessage>(Action<TMessage> listener, int priority = 0)
			where TMessage : IMessage;

		void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage;

		void Message<TMessage>(TMessage message)
			where TMessage : IMessage;
	}
}
