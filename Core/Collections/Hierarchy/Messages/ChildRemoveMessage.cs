using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	class ChildRemoveMessage<T> : KeyValueMessage<T, int, T>, IChildRemoveMessage<T>
		where T : IMessenger, IHierarchy<T>
	{
		public ChildRemoveMessage(int key, T value) : base(key, value) { }
	}
}