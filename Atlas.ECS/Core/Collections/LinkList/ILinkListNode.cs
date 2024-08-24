namespace Atlas.Core.Collections.LinkedList;
public interface ILinkListNode<T>
{
	public IReadOnlyLinkList<T> List { get; }

	public ILinkListNode<T> Previous { get; }

	public ILinkListNode<T> Next { get; }

	public T Value { get; }
}