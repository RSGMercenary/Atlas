using System;

namespace Atlas.Core.Messages
{
	public interface IMessenger : IDisposable
	{

	}

	public interface IMessenger<in T> : IMessenger
		where T : IMessenger
	{
		void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T>;

		void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<T>;

		void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T>;

		void Message<TMessage>(TMessage message)
			where TMessage : IMessage<T>;
	}
}