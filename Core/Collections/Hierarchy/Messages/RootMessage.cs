using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	class RootMessage<T> : PropertyMessage<T, T>, IRootMessage<T>
		where T : class, IMessenger, IHierarchy<T>
	{
		public RootMessage(T current, T previous) : base(current, previous) { }
	}
}