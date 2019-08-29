namespace Atlas.Core.Messages
{
	class ChildAddMessage<T> : KeyValueMessage<T, int, T>, IChildAddMessage<T>
		where T : IMessenger, IHierarchy<T>
	{
		public ChildAddMessage(int key, T value) : base(key, value)
		{
		}
	}
}