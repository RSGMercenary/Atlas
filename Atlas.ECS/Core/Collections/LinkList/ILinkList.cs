namespace Atlas.Core.Collections.LinkList;

public interface ILinkList<T> : ILinkListIterator<T>
{
	new T this[int index] { get; set; }

	int GetIndex(T value);

	ILinkListNode<T> GetNode(T value);

	bool Contains(T value);
}