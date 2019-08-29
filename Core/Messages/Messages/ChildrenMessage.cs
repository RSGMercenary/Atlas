namespace Atlas.Core.Messages
{
	class ChildrenMessage<T> : Message<T>, IChildrenMessage<T>
				where T : IMessenger, IHierarchy<T>

	{
		public ChildrenMessage()
		{

		}
	}
}