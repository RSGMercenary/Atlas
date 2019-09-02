using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	class ChildAddMessage<T> : KeyValueMessage<T, int, T>, IChildAddMessage<T>
		where T : IMessenger, IHierarchy<T>
	{
		public ChildAddMessage(int key, T value) : base(key, value) { }
	}
}