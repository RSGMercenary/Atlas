using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	class ParentMessage<T> : PropertyMessage<T, T>, IParentMessage<T>
		where T : IMessenger, IHierarchy<T>

	{
		public ParentMessage(T current, T previous) : base(current, previous) { }
	}
}