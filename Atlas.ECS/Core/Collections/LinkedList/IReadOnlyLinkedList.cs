using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkedList;

public interface IReadOnlyLinkedList<T> : IEnumerable<ILinkedListNode<T>>, IDisposable
{
	int Count { get; }

	ILinkedListNode<T> First { get; }

	ILinkedListNode<T> Last { get; }

	ILinkedListNode<T> Get(int index);

	T this[int index] { get; }

	bool Contains(T value);

	IEnumerable<ILinkedListNode<T>> Enumerate(bool forward = true);
}