using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IChildrenMessage<out T> : IMessage<T> where T : IMessenger, IHierarchy<T> { }
}