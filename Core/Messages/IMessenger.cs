using Atlas.Core.Collections.Hierarchy;
using System;

namespace Atlas.Core.Messages
{
	public interface IMessenger : IDisposable
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

	public interface IHierarchyMessenger<T> : IMessenger, IHierarchy<T>
		where T : IHierarchy<T>, IMessenger
	{
		void AddListener<TMessage>(Action<TMessage> listener, MessageFlow flow)
			where TMessage : IMessage;

		void AddListener<TMessage>(Action<TMessage> listener, int priority, MessageFlow flow)
			where TMessage : IMessage;

		void Message<TMessage>(TMessage message, MessageFlow flow)
			where TMessage : IMessage;
	}
}
