using Atlas.Core.Collections.Pool;
using Atlas.Core.Extensions;
using System;
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

	#region Sort
	#region Insertion
	public void InsertionSort(Func<T, T, int> compare)
	{
		if(first == last)
		{
			return;
		}
		var remains = first.next;
		for(var node = remains; node != null; node = remains)
		{
			remains = node.next;
			LinkListNode<T> other;
			for(other = node.previous; other != null; other = other.previous)
			{
				if(compare(node.data.value, other.data.value) >= 0)
				{
					// move node to after other
					if(node != other.next)
					{
						// remove from place
						if(last == node)
						{
							last = node.previous;
						}
						node.previous.next = node.next;
						if(node.next != null)
						{
							node.next.previous = node.previous;
						}
						// insert after other
						node.next = other.next;
						node.previous = other;
						node.next.previous = node;
						other.next = node;
					}
					break; // exit the inner for loop
				}
			}
			if(other == null) // the node belongs at the start of the list
			{
				// remove from place
				if(last == node)
				{
					last = node.previous;
				}
				node.previous.next = node.next;
				if(node.next != null)
				{
					node.next.previous = node.previous;
				}
				// insert at head
				node.next = first;
				first.previous = node;
				node.previous = null;
				first = node;
			}
		}
	}
	#endregion

	#region Merge
	public void MergeSort(Func<T, T, int> compare)
	{
		if(first == last)
			return;

		var lists = new List<LinkListNode<T>>();
		// disassemble the list
		var start = first;
		LinkListNode<T> end;
		while(start != null)
		{
			end = start;
			while(end.next != null && compare(end.data.value, end.next.data.value) <= 0)
				end = end.next;

			var next = end.next;
			start.previous = end.next = null;
			lists.Add(start);
			start = next;
		}

		// reassemble it in order
		while(lists.Count > 1)
			lists.Add(Merge(lists.Shift(), lists.Shift(), compare));

		// find the tail
		last = first = lists[0];

		while(last.next != null)
			last = last.next;
	}

	private LinkListNode<T> Merge(LinkListNode<T> head1, LinkListNode<T> head2, Func<T, T, int> compare)
	{
		LinkListNode<T> node;
		LinkListNode<T> head;
		if(compare(head1.data.value, head2.data.value) <= 0)
		{
			head = node = head1;
			head1 = head1.next;
		}
		else
		{
			head = node = head2;
			head2 = head2.next;
		}
		while(head1 != null && head2 != null)
		{
			if(compare(head1.data.value, head2.data.value) <= 0)
			{
				node.next = head1;
				head1.previous = node;
				node = head1;
				head1 = head1.next;
			}
			else
			{
				node.next = head2;
				head2.previous = node;
				node = head2;
				head2 = head2.next;
			}
		}
		if(head1 != null)
		{
			node.next = head1;
			head1.previous = node;
		}
		else
		{
			node.next = head2;
			head2.previous = node;
		}
		return head;
	}
	#endregion
	#endregion

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
		++node.data.iterators;

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

		++count;
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

		--count;
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