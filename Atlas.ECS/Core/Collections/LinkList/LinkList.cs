using Atlas.Core.Collections.Pool;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkedList;

public class LinkList<T> : ReadOnlyLinkList<T>, ILinkList<T>
{
	public LinkList()
	{
		PoolManager.Instance.AddPool(() => new ReadOnlyLinkList<T>());
		PoolManager.Instance.AddPool(() => new LinkListNode<T>());
		PoolManager.Instance.AddPool(() => new LinkListData<T>());
	}

	~LinkList()
	{
		PoolManager.Instance.RemovePool<ReadOnlyLinkList<T>>();
		PoolManager.Instance.RemovePool<LinkListNode<T>>();
		PoolManager.Instance.RemovePool<LinkListData<T>>();
	}

	public IReadOnlyLinkList<T> GetIterator()
	{
		var enumerator = PoolManager.Instance.Get<ReadOnlyLinkList<T>>();
		enumerator.Copy(this);
		return enumerator;
	}

	#region Add
	public bool Add(T value) => Add(value, count);

	public bool Add(T value, int index)
	{
		if(index < 0 || index > count)
			return false;

		var node = PoolManager.Instance.Get<LinkListNode<T>>();
		node.data = PoolManager.Instance.Get<LinkListData<T>>();
		node.data.removed = false;
		node.data.value = value;
		node.data.iterators++;

		return AddNode(node, index);
	}

	public bool AddNode(LinkListNode<T> node, int index)
	{
		if(node == null || (node.list != null && node.list != this))
			return false;

		if(node.list == this)
			ListSetNode(node, index);
		else
			ListAddNode(node, index);
		return true;
	}

	private void ListAddNode(LinkListNode<T> node, int index)
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

	public bool RemoveNode(LinkListNode<T> node)
	{
		if(node?.list != this)
			return false;

		ListRemoveNode(node);

		node.data.removed = true;
		node.Dispose();

		return true;
	}

	private void ListRemoveNode(LinkListNode<T> node)
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

	public bool SetNode(ILinkListNode<T> node, int index)
	{
		var current = node as LinkListNode<T>;

		if(current?.list != this)
			return false;

		ListSetNode(current, index);
		return true;
	}

	private void ListSetNode(LinkListNode<T> node, int index)
	{
		ListRemoveNode(node);
		ListAddNode(node, index);
	}
	#endregion

	#region Swap
	public bool Swap(T value1, T value2) => SwapNodes(GetNode(value1), GetNode(value2));

	public bool Swap(int index1, int index2) => SwapNodes(GetNode(index1), GetNode(index2));

	public bool SwapNodes(ILinkListNode<T> node1, ILinkListNode<T> node2)
	{
		var current1 = node1 as LinkListNode<T>;
		var current2 = node2 as LinkListNode<T>;

		if(current1?.list != this || current2?.list != this)
			return false;

		if(current1 == first)
			first = current2;
		else if(current2 == first)
			first = current1;

		if(current1 == last)
			last = current2;
		else if(current2 == last)
			last = current1;

		var temp = current1.next;
		current1.next = current2.next;
		current2.next = temp;

		if(current1.next != null)
			current1.next.previous = current1;
		if(current2.next != null)
			current2.next.previous = current2;

		temp = current1.previous;
		current1.previous = current2.previous;
		current2.previous = temp;

		if(current1.previous != null)
			current1.previous.next = current1;
		if(current2.previous != null)
			current2.previous.next = current2;

		return true;
	}
	#endregion

	public override IEnumerable<ILinkListNode<T>> Enumerate(bool forward = true)
	{
		using var list = GetIterator();
		return list.Enumerate(forward);
	}

	public LinkList<T> Clone()
	{
		var clone = new LinkList<T>();
		clone.Copy(this);
		return clone;
	}
}