using Atlas.Core.Collections.Pool;

namespace Atlas.Core.Collections.LinkList;

internal class LinkListData<T>
{
	public bool removed = false;
	public int iterators = 0;
	public T value;

	internal LinkListData() { }

	public void Dispose()
	{
		if(--iterators > 0)
			return;
		value = default;
		PoolManager.Instance.Put(this);
	}
}