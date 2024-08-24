using Atlas.Core.Collections.Pool;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.Core.Collections.LinkedList;
public class ReadOnlyLinkList<T> : IReadOnlyLinkList<T>
{
	internal int count = 0;
	internal LinkListNode<T> first;
	internal LinkListNode<T> last;

	internal ReadOnlyLinkList() { }

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
	}

	public int Count => count;

	public ILinkListNode<T> First => first;

	public ILinkListNode<T> Last => last;

	internal virtual void Copy(LinkList<T> list)
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
		copy.data.iterators++;
		count++;
	}

	#region Get
	public T this[int index]
	{
		get
		{
			var node = GetNode(index);
			return node != null ? node.data.value : default;
		}
	}

	public ILinkListNode<T> Get(int index) => GetNode(index);

	protected LinkListNode<T> GetNode(T value)
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

	protected LinkListNode<T> GetNode(int index)
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
	public IEnumerable<ILinkListNode<T>> Forward()
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

	public IEnumerable<ILinkListNode<T>> Backward()
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

	public virtual IEnumerable<ILinkListNode<T>> Enumerate(bool forward = true)
	{
		return forward ? Forward() : Backward();
	}

	public IEnumerator<ILinkListNode<T>> GetEnumerator() => Forward().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	public override string ToString()
	{
		return "[" + string.Join(", ", Forward()) + "]";
	}
}