using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	class ChildrenMessage<T> : Message<T>, IChildrenMessage<T>
		where T : IMessenger, IHierarchy<T>
	{
		public ChildrenMessage() { }
	}
}