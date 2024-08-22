using Atlas.Core.Collections.Pool;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkedList;

public class LinkedList<T> : ReadOnlyLinkedList<T>, ILinkedList<T>
{
	private int iterators = 0;
	private readonly Stack<LinkedListData<T>> removed = new();

	public LinkedList()
	{
		PoolManager.Instance.AddPool(() => new ReadOnlyLinkedList<T>());
		PoolManager.Instance.AddPool(() => new LinkedListNode<T>());
		PoolManager.Instance.AddPool(() => new LinkedListData<T>());
	}

	~LinkedList()
	{
		PoolManager.Instance.RemovePool<ReadOnlyLinkedList<T>>();
		PoolManager.Instance.RemovePool<LinkedListNode<T>>();
		PoolManager.Instance.RemovePool<LinkedListData<T>>();
	}

	public override void Dispose()
	{
		RemoveNodes();
	}

	public IReadOnlyLinkedList<T> Clone() => PoolManager.Instance.Get<ReadOnlyLinkedList<T>>().Clone(this);

	internal void AddIterator() => iterators++;

	internal void RemoveIterator()
	{
		if(--iterators == 0)
		{
			while(removed.TryPop(out var data))
				data.Dispose();
		}
	}

	#region Add
	public void Add(T value) => Add(value, count);

	public bool Add(T value, int index)
	{
		if(index < 0 || index > count)
			return false;

		var node = PoolManager.Instance.Get<LinkedListNode<T>>();
		node.data = PoolManager.Instance.Get<LinkedListData<T>>();
		node.data.value = value;

		return AddNode(node, index);
	}

	public bool AddNode(LinkedListNode<T> node, int index)
	{
		if(node == null || (node.list != null && node.list != this))
			return false;

		if(node.list == this)
			ListSetNode(node, index);
		else
			ListAddNode(node, index);
		return true;
	}

	private void ListAddNode(LinkedListNode<T> node, int index)
	{
		node.list = this;

		if(count == 0)
		{
			first = node;
			last = node;
		}
		else
		{
			if(index == 0)
			{
				first.previous = node;
				first.previous.next = first;
				first = node;
			}
			else if(index == count)
			{
				last.next = node;
				last.next.previous = last;
				last = node;
			}
			else
			{
				var current = GetNode(index);

				node.previous = current.previous;
				node.next = current;

				current.previous.next = node;
				current.previous = node;
			}
		}

		count++;
	}

	#endregion

	#region Remove
	public bool Remove(T value) => RemoveNode(GetNode(value));

	public bool Remove(int index) => RemoveNode(GetNode(index));

	public bool RemoveAll() => RemoveNodes();

	public bool RemoveNode(LinkedListNode<T> node)
	{
		if(node?.list != this)
			return false;

		ListRemoveNode(node);

		if(iterators > 0)
		{
			node.data.removed = true;
			removed.Push(node.data);
		}
		else
			node.data.Dispose();

		node.Dispose();

		return true;
	}

	private void ListRemoveNode(LinkedListNode<T> node)
	{
		node.list = null;

		if(node == first)
		{
			first = first.next;
			if(first != null)
				first.previous = null;
			else
				last = null;
		}
		else if(node == last)
		{
			last = last.previous;
			if(last != null)
				last.next = null;
			else
				first = null;
		}
		else
		{
			node.previous.next = node.next;
			node.next.previous = node.previous;
		}

		count--;
	}

	private bool RemoveNodes()
	{
		if(last == null)
			return false;
		while(last != null)
			RemoveNode(last);
		return true;
	}
	#endregion

	#region Get / Set
	public new T this[int index]
	{
		get => GetNode(index).data.value;
		set => GetNode(index).data.value = value;
	}

	public bool SetNode(ILinkedListNode<T> node, int index)
	{
		var current = node as LinkedListNode<T>;
		if(current?.list != this)
			return false;
		ListSetNode(current, index);
		return true;
	}

	private void ListSetNode(LinkedListNode<T> node, int index)
	{
		ListRemoveNode(node);
		ListAddNode(node, index);
	}
	#endregion

	#region Swap
	public bool Swap(T value1, T value2) => SwapNodes(GetNode(value1), GetNode(value2));

	public bool Swap(int index1, int index2) => SwapNodes(GetNode(index1), GetNode(index2));

	private static bool SwapNodes(LinkedListNode<T> node1, LinkedListNode<T> node2)
	{
		if(node1 == null || node2 == null)
			return false;
		(node2.data, node1.data) = (node1.data, node2.data);
		return true;
	}
	#endregion

	public override IEnumerable<ILinkedListNode<T>> Enumerate(bool forward = true)
	{
		using var list = Clone();
		return list.Enumerate(forward);
	}
}