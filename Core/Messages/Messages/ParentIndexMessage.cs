namespace Atlas.Core.Messages
{
	class ParentIndexMessage<T> : PropertyMessage<T, int>, IParentIndexMessage<T>
				where T : IMessenger, IHierarchy<T>

	{
		public ParentIndexMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}