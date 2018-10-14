using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class AutoDisposeMessage<TMessenger> : PropertyMessage<TMessenger, bool>, IAutoDisposeMessage<TMessenger>
		where TMessenger : IAutoDispose
	{
		public AutoDisposeMessage(TMessenger messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
