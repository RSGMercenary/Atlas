using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	class ParentIndexMessage<T> : PropertyMessage<T, int>, IParentIndexMessage<T>
		where T : class, IMessenger, IHierarchy<T>

	{
		public ParentIndexMessage(int current, int previous) : base(current, previous) { }
	}
}