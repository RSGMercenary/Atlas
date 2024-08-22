using Atlas.Core.Messages;
using System;

namespace Atlas.Core.Collections.Hierarchy;
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