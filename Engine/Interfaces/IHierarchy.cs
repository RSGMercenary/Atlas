using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Signals;

namespace Atlas.Engine.Interfaces
{
	interface IHierarchy<T>
	{
		T Root { get; }

		T Parent { get; set; }

		IReadOnlyLinkList<T> Children { get; }

		bool SetParent(T parent = default(T), int index = int.MaxValue);

		bool HasDescendant(T descendant);

		bool HasAncestor(T ancestor);

		bool HasChild(T child);

		int GetChildIndex(T child);

		bool SetChildIndex(T child, int index);

		T GetChild(int index);

		T AddChild(T child);

		T AddChild(T child, int index);

		T RemoveChild(T child);

		T RemoveChild(int index);

		bool RemoveChildren();

		ISignal<T, T, T> ParentChanged { get; }

		ISignal<T, int, int> ParentIndexChanged { get; }

		//Bool is true for inclusive (1 through 4) and false for exclusive (1 and 4)
		ISignal<T, int, int, bool> ChildIndicesChanged { get; }

		ISignal<T, T, int> ChildAdded { get; }

		ISignal<T, T, int> ChildRemoved { get; }
	}
}