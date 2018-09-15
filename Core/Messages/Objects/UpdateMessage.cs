using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateMessage : PropertyMessage<IUpdateObject, bool>, IUpdateMessage
	{
		public UpdateMessage(IUpdateObject messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
