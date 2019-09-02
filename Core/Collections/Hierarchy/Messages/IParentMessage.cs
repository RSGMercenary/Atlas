using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IParentMessage<out T> : IPropertyMessage<T, T> where T : IMessenger, IHierarchy<T> { }
}