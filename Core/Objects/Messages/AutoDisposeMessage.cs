using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class AutoDisposeMessage : PropertyMessage<IAutoDispose, bool>, IAutoDisposeMessage
	{
		public AutoDisposeMessage(IAutoDispose messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
