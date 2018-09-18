using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class AutoDestroyMessage<TMessenger> : PropertyMessage<TMessenger, bool>, IAutoDestroyMessage<TMessenger>
		where TMessenger : IAutoDestroy
	{
		public AutoDestroyMessage(TMessenger messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
