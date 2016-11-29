using System;

namespace Atlas.Engine.Collections.LinkList
{
	interface ILinkList<T>:IReadOnlyLinkList<T>, IDisposable
	{
		T Add(T data);
		T Add(T data, int index);
		T Remove(T data);
		T Remove(int index);
		bool SetIndex(T data, int index);

		new T this[int i] { get; set; }
	}
}
