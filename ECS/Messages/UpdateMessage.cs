using Atlas.ECS.Objects;

namespace Atlas.Framework.Messages
{
	class UpdateMessage : PropertyMessage<IUpdateObject, bool>, IUpdateMessage
	{
		public UpdateMessage(bool current, bool previous) : base(current, previous)
		{
		}
	}
}
