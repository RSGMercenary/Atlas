namespace Atlas.Core.Collections.LinkList;
public interface ILinkListNode<out T>
{
	public IReadOnlyLinkList<T> List { get; }

	public ILinkListNode<T> Previous { get; }

	public ILinkListNode<T> Next { get; }

	public T Value { get; }
}