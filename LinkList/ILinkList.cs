using Atlas.Interfaces;

namespace Atlas.LinkList
{
	interface ILinkList<T>:IReadOnlyLinkList<T>, IDispose
	{
		T Add(T data);
		T Add(T data, int index);
		T Remove(T data);
		T Remove(int index);
		bool SetIndex(T data, int index);
	}
}
