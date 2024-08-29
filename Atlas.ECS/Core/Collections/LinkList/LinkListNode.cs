using Atlas.Core.Collections.Pool;

namespace Atlas.Core.Collections.LinkList;

public class LinkListNode<T> : ILinkListNode<T>
{
	internal ReadOnlyLinkList<T> list;
	internal LinkListNode<T> previous;
	internal LinkListNode<T> next;
	internal LinkListData<T> data;

	internal LinkListNode() { }

	public void Dispose()
	{
		list = null;
		previous = null;
		next = null;
		data.Dispose();
		data = null;
		PoolManager.Instance.Put(this);
	}

	public IReadOnlyLinkList<T> List => list;

	public ILinkListNode<T> Previous
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

	public ILinkListNode<T> Next
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

	public override string ToString() => data.value.ToString();
}