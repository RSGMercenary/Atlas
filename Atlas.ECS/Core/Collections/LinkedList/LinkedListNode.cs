using Atlas.Core.Collections.Pool;

namespace Atlas.Core.Collections.LinkedList;

public class LinkedListNode<T> : ILinkedListNode<T>
{
	internal ReadOnlyLinkedList<T> list;
	internal LinkedListNode<T> previous;
	internal LinkedListNode<T> next;
	internal LinkedListData<T> data;

	internal LinkedListNode() { }

	public void Dispose()
	{
		list = null;
		previous = null;
		next = null;
		data = null;
		PoolManager.Instance.Put(this);
	}

	public ILinkedListNode<T> Previous
	{
		get
		{
			var current = previous;
			while(current != null)
			{
				if(!current.data.removed)
					break;
				current = current.previous;
			}
			return current;
		}
	}

	public ILinkedListNode<T> Next
	{
		get
		{
			var current = next;
			while(current != null)
			{
				if(!current.data.removed)
					break;
				current = current.next;
			}
			return current;
		}
	}

	public T Value => data.value;

	public override string ToString() => Value.ToString();
}