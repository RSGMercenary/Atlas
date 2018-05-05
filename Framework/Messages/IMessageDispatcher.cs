using System;

namespace Atlas.Framework.Messages
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

	public interface IMessageDispatcher<TMessenger> : IMessageDispatcher
		where TMessenger : IMessageDispatcher<TMessenger>
	{
		new void AddListener<TMessage>(Action<TMessage> listener, int priority = 0)
			where TMessage : IMessage<TMessenger>;

		new void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<TMessenger>;

		new void Message<TMessage>(TMessage message)
			where TMessage : IMessage<TMessenger>;
	}
}
