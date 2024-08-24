namespace Atlas.Core.Collections.LinkedList;

public interface ILinkList<T> : IReadOnlyLinkList<T>
{
	new T this[int index] { get; set; }
}