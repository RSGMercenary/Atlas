using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class AutoDisposeMessage<T> : PropertyMessage<T, bool>, IAutoDisposeMessage<T>
		where T : IAutoDispose
	{
		public AutoDisposeMessage(T messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
