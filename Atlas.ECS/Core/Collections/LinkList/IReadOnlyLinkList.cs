using System;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkedList;

public interface IReadOnlyLinkList<T> : IEnumerable<ILinkListNode<T>>, IDisposable
{
	int Count { get; }

	ILinkListNode<T> First { get; }

	ILinkListNode<T> Last { get; }

	ILinkListNode<T> Get(int index);

	T this[int index] { get; }

	bool Contains(T value);

	IEnumerable<ILinkListNode<T>> Forward();

	IEnumerable<ILinkListNode<T>> Backward();

	IEnumerable<ILinkListNode<T>> Enumerate(bool forward = true);
}