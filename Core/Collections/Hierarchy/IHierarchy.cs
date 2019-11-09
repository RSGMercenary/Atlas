using Atlas.Core.Collections.Group;
using Atlas.Core.Messages;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IReadOnlyHierarchy<T> : IMessenger<T>, IEnumerable<T>
		where T : IReadOnlyHierarchy<T>
	{
		T Root { get; }

		bool IsRoot { get; }

		T Parent { get; }

		int ParentIndex { get; }

		IReadOnlyGroup<T> Children { get; }
		T GetChild(int index);
		int GetChildIndex(T child);

		bool HasDescendant(T descendant);
		bool HasAncestor(T ancestor);
		bool HasChild(T child);
		bool HasSibling(T sibling);

		void AddListener<TMessage>(Action<TMessage> listener, Relation flow)
			where TMessage : IMessage<T>;

		void AddListener<TMessage>(Action<TMessage> listener, int priority, Relation flow)
			where TMessage : IMessage<T>;

		void Message<TMessage>(TMessage message, Relation flow)
			where TMessage : IMessage<T>;
	}

	public interface IHierarchy<T> : IReadOnlyHierarchy<T>
		where T : IHierarchy<T>
	{
		new bool IsRoot { get; set; }

		new T Parent { get; set; }
		new int ParentIndex { get; set; }
		T SetParent(T parent);
		T SetParent(T parent, int index);

		T AddChild(T child);
		T AddChild(T child, int index);

		T RemoveChild(T child);
		T RemoveChild(int index);
		bool RemoveChildren();

		bool SetChildIndex(T child, int index);
		bool SwapChildren(T child1, T child2);
		bool SwapChildren(int index1, int index2);
	}
}