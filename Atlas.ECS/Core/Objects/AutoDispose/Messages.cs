using Atlas.Core.Messages;

namespace Atlas.Core.Objects.AutoDispose;

public interface IAutoDisposeMessage<out T> : IPropertyMessage<T, bool> where T : IAutoDispose, IMessenger { }

class AutoDisposeMessage<T> : PropertyMessage<T, bool>, IAutoDisposeMessage<T> where T : IAutoDispose, IMessenger
{
	public AutoDisposeMessage(bool current, bool previous) : base(current, previous) { }
}