using Atlas.Core.Collections.Pool;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkList;
public class ReadOnlyLinkList<T> : IReadOnlyLinkList<T>, ILinkListIterator<T>
{
	internal int count = 0;
	internal LinkListNode<T> first;
	internal LinkListNode<T> last;

	internal ReadOnlyLinkList() { }

	public virtual void Dispose() => RemoveNodes();

	public int Count => count;

	public ILinkListNode<T> First => first;

	public ILinkListNode<T> Last => last;

	internal void Copy(LinkList<T> list)
	{
		if(list.count <= 0)
			return;

		var copy = PoolManager.Instance.Get<LinkListNode<T>>();
		var source = list.first;

		Copy(source, copy);

		first = copy;

		source = source.next;
		while(source != null)
		{
			if(!source.data.removed)
			{
				copy.next = PoolManager.Instance.Get<LinkListNode<T>>();
				copy.next.previous = copy;
				copy = copy.next;

				Copy(source, copy);
			}

			source = source.next;
		}

		last = copy;
	}

	private void Copy(LinkListNode<T> source, LinkListNode<T> copy)
	{
		copy.data = source.data;
		++copy.data.iterators;
		++count;
	}

	#region Get
	public T this[int index] => InternalGetNode(index).data.value;

	public int GetIndex(T value)
	{
		if(value == null)
			return -1;

		var nodeIndex = 0;
		var node = first;
		while(node != null)
		{
			if(!node.data.removed)
			{
				if(node.data.value.Equals(value))
					return nodeIndex;
				++nodeIndex;
			}
			node = node.next;

		}
		return -1;
	}

	public ILinkListNode<T> GetNode(int index) => InternalGetNode(index);

	public ILinkListNode<T> GetNode(T value) => InternalGetNode(value);

	protected LinkListNode<T> InternalGetNode(T value)
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

	protected LinkListNode<T> InternalGetNode(int index)
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
				++nodeIndex;
			}
			node = node.next;
		}
		return null;
	}

	public bool Contains(T value) => InternalGetNode(value) != null;
	#endregion

	protected bool RemoveNodes()
	{
		if(last == null)
			return false;

		var current = last;
		do
		{
			var node = current.previous;
			current.Dispose();
			current = node;
		}
		while(current != null);

		first = null;
		last = null;
		count = 0;
		return true;
	}

	#region Iterate
	public virtual IEnumerable<T> Forward()
	{
		if(first == null)
			yield break;

		var node = first;

		while(node != null)
		{
			if(!node.data.removed)
				yield return node.data.value;
			node = node.next;
		}
	}

	public virtual IEnumerable<T> Backward()
	{
		if(last == null)
			yield break;

		var node = last;

		while(node != null)
		{
			if(!node.data.removed)
				yield return node.data.value;
			node = node.previous;
		}
	}

	public IEnumerable<T> Enumerate(bool forward = true)
	{
		return forward ? Forward() : Backward();
	}

	public IEnumerator<T> GetEnumerator() => Forward().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	public override string ToString()
	{
		return "[" + string.Join(", ", Forward()) + "]";
	}
}