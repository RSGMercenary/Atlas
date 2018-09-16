using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class AutoDestroyMessage<TMessenger> : PropertyMessage<TMessenger, bool>, IAutoDestroyMessage<TMessenger>
		where TMessenger : IAutoDestroyObject
	{
		public AutoDestroyMessage(TMessenger messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
