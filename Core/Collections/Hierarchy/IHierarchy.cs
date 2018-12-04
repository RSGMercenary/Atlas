using Atlas.Core.Collections.Group;
using Atlas.Core.Messages;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IReadOnlyHierarchy<T> : IEnumerable<T>
		where T : IReadOnlyHierarchy<T>, IMessenger
	{
		T Root { get; }
		int RootIndex { get; }

		T Parent { get; }
		int ParentIndex { get; }

		IReadOnlyGroup<T> Children { get; }
		T GetChild(int index);
		int GetChildIndex(T child);

		bool HasDescendant(T descendant);
		bool HasAncestor(T ancestor);
		bool HasChild(T child);
		bool HasSibling(T sibling);
	}

	public interface IHierarchy<T> : IReadOnlyHierarchy<T>, IDisposable
		where T : IHierarchy<T>, IMessenger
	{
		new T Parent { get; set; }
		new int ParentIndex { get; set; }
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
