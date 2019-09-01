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

		bool RemoveListeners();

		void Message<TMessage>(TMessage message)
			where TMessage : IMessage<TMessenger>;
	}

	public interface IHierarchyMessenger<TMessenger> : IMessenger<TMessenger>, IHierarchy<TMessenger>
		where TMessenger : IMessenger, IHierarchy<TMessenger>
	{
		void AddListener<TMessage>(Action<TMessage> listener, MessageFlow flow)
			where TMessage : IMessage<TMessenger>;

		void AddListener<TMessage>(Action<TMessage> listener, int priority, MessageFlow flow)
			where TMessage : IMessage<TMessenger>;

		void Message<TMessage>(TMessage message, MessageFlow flow)
			where TMessage : IMessage<TMessenger>;
	}
}