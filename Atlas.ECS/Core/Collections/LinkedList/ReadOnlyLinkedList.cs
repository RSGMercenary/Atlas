using Atlas.Core.Collections.Pool;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkedList;

public class ReadOnlyLinkedList<T> : IReadOnlyLinkedList<T>
{
	internal int count = 0;
	internal LinkedListNode<T> first;
	internal LinkedListNode<T> last;
	internal LinkedList<T> source;

	internal ReadOnlyLinkedList() { }

	internal ReadOnlyLinkedList<T> Clone(LinkedList<T> list)
	{
		this.source = list;
		this.source.AddIterator();

		if(list.count <= 0)
			return this;

		var current = PoolManager.Instance.Get<LinkedListNode<T>>();
		var source = list.first;

		current.data = source.data;
		source = source.next;

		first = current;

		while(source != null)
		{
			if(!source.data.removed)
			{
				var node = PoolManager.Instance.Get<LinkedListNode<T>>();
				node.data = source.data;

				current.next = node;
				current.next.previous = current;
				current = node;

				count++;
			}

			source = source.next;
		}

		last = current;

		return this;
	}

	public virtual void Dispose()
	{
		if(last != null)
		{
			var current = last.previous;
			last.Dispose();
			while(current != null)
			{
				var node = current.previous;
				current.Dispose();
				current = node;
			}
		}

		first = null;
		last = null;
		count = 0;

		source.RemoveIterator();
		source = null;

		PoolManager.Instance.Put(this);
	}

	public int Count => count;

	public ILinkedListNode<T> First => first;

	public ILinkedListNode<T> Last => last;

	#region Get
	public T this[int index]
	{
		get { var node = GetNode(index); return node != null ? node.data.value : default; }
	}

	public ILinkedListNode<T> Get(int index) => GetNode(index);

	protected LinkedListNode<T> GetNode(T value)
	{
		if(value == null)
			return null;

		var node = first;
		while(node != null)
		{
			if(!node.data.removed)
			{
				if(node.data.value.Equals(value))
					return node;
			}
			node = node.next;
		}
		return null;
	}

	protected LinkedListNode<T> GetNode(int index)
	{
		if(index <= -1 || index >= count)
			return null;

		var nodeIndex = 0;
		var node = first;
		while(node != null)
		{
			if(!node.data.removed)
			{
				if(nodeIndex == index)
					return node;
				nodeIndex++;
			}
			node = node.next;

		}
		return null;
	}

	public bool Contains(T value) => GetNode(value) != null;
	#endregion

	#region Iterate
	public IEnumerable<ILinkedListNode<T>> Forward()
	{
		if(first == null)
			yield break;

		var node = first;

		while(node != null)
		{
			if(!node.data.removed)
				yield return node;
			node = node.next;
		}
	}

	public IEnumerable<ILinkedListNode<T>> Backward()
	{
		if(last == null)
			yield break;

		var node = last;

		while(node != null)
		{
			if(!node.data.removed)
				yield return node;
			node = node.previous;
		}
	}

	public virtual IEnumerable<ILinkedListNode<T>> Enumerate(bool forward = true)
	{
		return forward ? Forward() : Backward();
	}

	public IEnumerator<ILinkedListNode<T>> GetEnumerator() => Forward().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	public override string ToString()
	{
		return "[" + string.Join(", ", Forward()) + "]";
	}
}