using Atlas.ECS.Objects;

namespace Atlas.Framework.Messages
{
	class UpdateMessage : PropertyMessage<IUpdateObject, bool>, IUpdateMessage
	{
		public UpdateMessage(IUpdateObject messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
