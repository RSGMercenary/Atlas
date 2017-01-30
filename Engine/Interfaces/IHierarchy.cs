using Atlas.Engine.Collections.Hierarchy;
using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Interfaces
{
	//interface IHierarchy<T, R> where R : T
	interface IHierarchy<T>:IDisposable
	{
		T Root { get; }

		T Parent { get; set; }

		int ParentIndex { get; set; }

		IReadOnlyLinkList<T> Children { get; }

		bool SetParent(T parent = default(T), int index = int.MaxValue);

		bool HasDescendant(T descendant);

		bool HasAncestor(T ancestor);

		bool HasChild(T child);

		int GetChildIndex(T child);

		bool SetChildIndex(T child, int index);

		bool SwapChildren(T child1, T child2);

		bool SwapChildren(int index1, int index2);

		T GetChild(int index);

		T AddChild(T child);

		T AddChild(T child, int index);

		T RemoveChild(T child);

		T RemoveChild(int index);

		bool RemoveChildren();

		ISignal<T, T, T, T> RootChanged { get; }

		ISignal<T, T, T, T> ParentChanged { get; }

		ISignal<T, int, int> ParentIndexChanged { get; }

		//-1 is for remove, +1 is for add, 0 is for swap.
		ISignal<T, int, int, HierarchyChange> ChildIndicesChanged { get; }

		ISignal<T, T, int> ChildAdded { get; }

		ISignal<T, T, int> ChildRemoved { get; }
	}
}