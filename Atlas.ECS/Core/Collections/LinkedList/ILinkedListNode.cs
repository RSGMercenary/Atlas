namespace Atlas.Core.Collections.LinkedList;
public interface ILinkedListNode<T>
{
	public ILinkedListNode<T> Previous { get; }

	public ILinkedListNode<T> Next { get; }

	public T Value { get; }
}