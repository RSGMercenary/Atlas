using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateMessage<TMessenger> : PropertyMessage<TMessenger, bool>, IUpdateMessage<TMessenger>
		where TMessenger : IUpdateObject
	{
		public UpdateMessage(TMessenger messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
