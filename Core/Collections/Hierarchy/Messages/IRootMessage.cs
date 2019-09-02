using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IRootMessage<out T> : IPropertyMessage<T, T> where T : IMessenger, IHierarchy<T> { }
}