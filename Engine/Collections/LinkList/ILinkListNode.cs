namespace Atlas.Engine.Collections.LinkList
{
	interface ILinkListNode<T>
	{
		ILinkList<T> LinkList { get; }
		ILinkListNode<T> Previous { get; }
		ILinkListNode<T> Next { get; }
		T Value { get; }
	}
}
