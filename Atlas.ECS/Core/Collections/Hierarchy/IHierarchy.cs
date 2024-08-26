using Atlas.Core.Collections.LinkList;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.Hierarchy;

public interface IReadOnlyHierarchy<T> : IEnumerable<T>
	where T : IReadOnlyHierarchy<T>
{
	event Action<T, T, T> RootChanged;
	event Action<T, T, T> ParentChanged;
	event Action<T, int, int> ParentIndexChanged;
	event Action<T, T, int> ChildAdded;
	event Action<T, T, int> ChildRemoved;
	event Action<T> ChildrenChanged;

	T Root { get; }

	bool IsRoot { get; }

	T Parent { get; }

	int ParentIndex { get; }

	IReadOnlyLinkList<T> Children { get; }
	T this[int index] { get; }
	T GetChild(int index);
	int GetChildIndex(T child);

	bool HasDescendant(T descendant);
	bool HasAncestor(T ancestor);
	bool HasChild(T child);
	bool HasSibling(T sibling);
}

public interface IHierarchy<T> : IReadOnlyHierarchy<T>
	where T : IHierarchy<T>
{
	new bool IsRoot { get; set; }

	new T Parent { get; set; }
	new int ParentIndex { get; set; }
	T SetParent(T parent);
	T SetParent(T parent, int index);

	new T this[int index] { get; set; }

	T AddChild(T child);
	T AddChild(T child, int index);

	T RemoveChild(T child);
	T RemoveChild(int index);
	bool RemoveChildren();

	bool SetChildIndex(T child, int index);
	bool SwapChildren(T child1, T child2);
	bool SwapChildren(int index1, int index2);
}