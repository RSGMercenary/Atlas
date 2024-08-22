using Atlas.Core.Collections.Pool;

namespace Atlas.Core.Collections.LinkedList;

public class LinkedListData<T>
{
	public bool removed = false;
	public T value;

	public LinkedListData() { }

	public void Dispose()
	{
		removed = false;
		value = default;
		PoolManager.Instance.Put(this);
	}
}