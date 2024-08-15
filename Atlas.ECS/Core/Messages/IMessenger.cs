using Atlas.Core.Collections.Hierarchy;
using Atlas.Signals.Slots;
using System;

namespace Atlas.Core.Messages;

public interface IMessenger : IDisposable { }

public interface IMessenger<TMessenger> : IMessenger
	where TMessenger : IMessenger
{
	ISlot<TMessage> AddListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<TMessenger>;

	ISlot<TMessage> AddListener<TMessage>(Action<TMessage> listener, int priority)
		where TMessage : IMessage<TMessenger>;

	bool RemoveListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<TMessenger>;

	bool RemoveListeners();

	ISlot<TMessage> GetListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<TMessenger>;

	bool HasListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<TMessenger>;

	void Message<TMessage>(TMessage message)
		where TMessage : IMessage<TMessenger>;
}

public interface IHierarchyMessenger<TMessenger> : IMessenger<TMessenger>, IHierarchy<TMessenger>
	where TMessenger : IMessenger, IHierarchy<TMessenger>
{
	void AddListener<TMessage>(Action<TMessage> listener, Relation flow)
		where TMessage : IMessage<TMessenger>;

	void AddListener<TMessage>(Action<TMessage> listener, int priority, Relation flow)
		where TMessage : IMessage<TMessenger>;

	void Message<TMessage>(TMessage message, Relation flow)
		where TMessage : IMessage<TMessenger>;
}