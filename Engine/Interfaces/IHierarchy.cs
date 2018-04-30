using Atlas.Engine.Collections.EngineList;

namespace Atlas.Engine.Interfaces
{
	public interface IHierarchy<T>
	{
		T Root { get; }

		T Parent { get; set; }

		int ParentIndex { get; set; }

		IReadOnlyEngineList<T> Children { get; }

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

		bool AddChildren(params T[] children);

		bool AddChildren(int index, params T[] children);

		T RemoveChild(T child);

		T RemoveChild(int index);

		bool RemoveChildren();
	}
}