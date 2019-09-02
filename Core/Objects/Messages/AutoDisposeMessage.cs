using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class AutoDisposeMessage<T> : PropertyMessage<T, bool>, IAutoDisposeMessage<T>
		where T : IAutoDispose, IMessenger
	{
		public AutoDisposeMessage(bool current, bool previous) : base(current, previous) { }
	}
}