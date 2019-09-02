using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IParentIndexMessage<out T> : IPropertyMessage<T, int> where T : IMessenger, IHierarchy<T> { }
}