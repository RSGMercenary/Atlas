using Atlas.Core.Collections.Pool;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkList;

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

		InternalAddNode(node, index);
		return true;
	}

	public bool AddNode(ILinkListNode<T> node, int index)
	{
		if(node == null || (node.List != null && node.List != this))
			return false;

		if(node.List == this)
			InternalSetNode((LinkListNode<T>)node, index);
		else
			InternalAddNode((LinkListNode<T>)node, index);
		return true;
	}

	private void InternalAddNode(LinkListNode<T> node, int index)
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
				var current = InternalGetNode(index);

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
	public bool Remove(T value) => InternalRemoveNode(InternalGetNode(value));

	public bool Remove(int index) => InternalRemoveNode(InternalGetNode(index));

	public bool RemoveAll() => RemoveNodes();

	public bool RemoveNode(ILinkListNode<T> node)
	{
		if(node?.List != this)
			return false;

		return InternalRemoveNode((LinkListNode<T>)node);
	}

	private bool InternalRemoveNode(LinkListNode<T> node, bool removed = true)
	{
		if(node == null)
			return false;

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

		node.previous = null;
		node.next = null;
		if(removed)
		{
			node.data.removed = true;
			node.Dispose();
		}

		count--;
		return true;
	}
	#endregion

	#region Get / Set
	public new T this[int index]
	{
		get => InternalGetNode(index).data.value;
		set => InternalGetNode(index).data.value = value;
	}

	public int Set(T value, int index)
	{
		var previous = GetIndex(value);
		if(previous > -1)
			InternalSetNode(InternalGetNode(previous), index);
		return previous;
	}

	public bool SetNode(ILinkListNode<T> node, int index)
	{
		if(node?.List != this)
			return false;

		InternalSetNode((LinkListNode<T>)node, index);
		return true;
	}

	private void InternalSetNode(LinkListNode<T> node, int index)
	{
		InternalRemoveNode(node, false);
		InternalAddNode(node, index);
	}
	#endregion

	#region Swap
	public bool Swap(T value1, T value2) => InternalSwapNodes(InternalGetNode(value1), InternalGetNode(value2));

	public bool Swap(int index1, int index2) => InternalSwapNodes(InternalGetNode(index1), InternalGetNode(index2));

	public bool SwapNodes(ILinkListNode<T> node1, ILinkListNode<T> node2)
	{
		if(node1?.List != this || node2?.List != this)
			return false;

		return InternalSwapNodes((LinkListNode<T>)node1, (LinkListNode<T>)node2);
	}

	protected bool InternalSwapNodes(LinkListNode<T> node1, LinkListNode<T> node2)
	{
		if(node1 == null || node2 == null)
			return false;

		if(node1 == first)
			first = node2;
		else if(node2 == first)
			first = node1;

		if(node1 == last)
			last = node2;
		else if(node2 == last)
			last = node1;

		var temp = node1.next;
		node1.next = node2.next;
		node2.next = temp;

		if(node1.next != null)
			node1.next.previous = node1;
		if(node2.next != null)
			node2.next.previous = node2;

		temp = node1.previous;
		node1.previous = node2.previous;
		node2.previous = temp;

		if(node1.previous != null)
			node1.previous.next = node1;
		if(node2.previous != null)
			node2.previous.next = node2;

		return true;
	}
	#endregion

	#region Iterate
	public override IEnumerable<T> Forward()
	{
		using var list = GetIterator();
		foreach(var value in list.Forward())
			yield return value;
	}

	public override IEnumerable<T> Backward()
	{
		using var list = GetIterator();
		foreach(var value in list.Backward())
			yield return value;
	}

	public ILinkListIterator<T> GetIterator()
	{
		var enumerator = PoolManager.Instance.Get<ReadOnlyLinkList<T>>();
		enumerator.Copy(this);
		return enumerator;
	}
	#endregion
}