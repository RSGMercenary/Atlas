using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkList;

public interface IReadOnlyLinkList<out T> : IEnumerable<T>
{
	int Count { get; }

	ILinkListNode<T> First { get; }

	ILinkListNode<T> Last { get; }

	ILinkListNode<T> GetNode(int index);

	T this[int index] { get; }

	IEnumerable<T> Forward();

	IEnumerable<T> Backward();

	IEnumerable<T> Enumerate(bool forward = true);
}