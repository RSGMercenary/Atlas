using Atlas.Core.Collections.Group;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IReadOnlyHierarchy
	{
		IReadOnlyHierarchy Root { get; }

		IReadOnlyHierarchy Parent { get; }

		int ParentIndex { get; }

		IReadOnlyGroup<IReadOnlyHierarchy> Children { get; }

		IReadOnlyHierarchy GetChild(int index);

		int GetChildIndex(IReadOnlyHierarchy child);

		bool HasDescendant(IReadOnlyHierarchy descendant);

		bool HasAncestor(IReadOnlyHierarchy ancestor);

		bool HasChild(IReadOnlyHierarchy child);

		bool HasSibling(IReadOnlyHierarchy sibling);
	}

	public interface IHierarchy : IReadOnlyHierarchy
	{
		new IHierarchy Parent { get; set; }

		new int ParentIndex { get; set; }

		new IReadOnlyGroup<IHierarchy> Children { get; }

		bool SetParent(IHierarchy parent, int index);

		bool SetChildIndex(IHierarchy child, int index);

		bool SwapChildren(IHierarchy child1, IHierarchy child2);

		bool SwapChildren(int index1, int index2);

		IHierarchy AddChild(IHierarchy child);

		IHierarchy AddChild(IHierarchy child, int index);

		bool AddChildren(params IHierarchy[] children);

		bool AddChildren(int index, params IHierarchy[] children);

		IHierarchy RemoveChild(IHierarchy child);

		IHierarchy RemoveChild(int index);

		bool RemoveChildren();
	}

	public interface IReadOnlyHierarchy<T> : IReadOnlyHierarchy
		where T : IReadOnlyHierarchy<T>
	{
		new T Root { get; }

		new T Parent { get; }

		new IReadOnlyGroup<T> Children { get; }

		new T GetChild(int index);

		int GetChildIndex(T child);

		bool HasDescendant(T descendant);

		bool HasAncestor(T ancestor);

		bool HasChild(T child);

		bool HasSibling(T sibling);
	}

	public interface IHierarchy<T> : IReadOnlyHierarchy<T>, IHierarchy
		where T : IHierarchy<T>
	{
		new T Parent { get; set; }

		bool SetParent(T parent, int index);

		bool SetChildIndex(T child, int index);

		bool SwapChildren(T child1, T child2);

		new T GetChild(int index);

		T AddChild(T child);

		T AddChild(T child, int index);

		bool AddChildren(params T[] children);

		bool AddChildren(int index, params T[] children);

		T RemoveChild(T child);

		new T RemoveChild(int index);
	}
}