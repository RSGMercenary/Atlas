namespace Atlas.Core.Messages
{
	class ParentMessage<T> : PropertyMessage<T, T>, IParentMessage<T>
				where T : IMessenger, IHierarchy<T>

	{
		public ParentMessage(T current, T previous) : base(current, previous)
		{
		}
	}
}