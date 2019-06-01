using System;

namespace Atlas.Core.Messages
{
	public interface IMessenger : IDisposable
	{

	}

	public interface IMessenger<TMessenger> : IMessenger
		where TMessenger : IMessenger
	{
		void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<TMessenger>;

		void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<TMessenger>;

		void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<TMessenger>;

		void Message<TMessage>(TMessage message)
			where TMessage : IMessage<TMessenger>;
	}
}