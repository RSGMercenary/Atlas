namespace Atlas.Core.Collections.LinkedList;

public interface ILinkedList<T> : IReadOnlyLinkedList<T>
{
	new T this[int index] { get; set; }

	IReadOnlyLinkedList<T> Clone();
}