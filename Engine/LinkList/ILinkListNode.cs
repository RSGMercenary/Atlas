namespace Atlas.Engine.LinkList
{
	interface ILinkListNode<T>
	{
		ILinkList<T> NodeList { get; }
		ILinkListNode<T> Previous { get; }
		ILinkListNode<T> Next { get; }
		T Value { get; }
	}
}
