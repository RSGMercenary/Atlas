namespace Atlas.Core.Messages
{
	class RootMessage<T> : PropertyMessage<T, T>, IRootMessage<T>
				where T : IMessenger, IHierarchy<T>

	{
		public RootMessage(T current, T previous) : base(current, previous)
		{
		}
	}
}