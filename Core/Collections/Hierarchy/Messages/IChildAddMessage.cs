using Atlas.Core.Messages;

namespace Atlas.Core.Collections.Hierarchy
{
	public interface IChildAddMessage<out T> : IKeyValueMessage<T, int, T> where T : IMessenger, IHierarchy<T> { }
}