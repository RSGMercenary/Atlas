using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateStateMessage : PropertyMessage<IUpdateState, TimeStep>, IUpdateStateMessage
	{
		public UpdateStateMessage(IUpdateState messenger, TimeStep current, TimeStep previous) : base(messenger, current, previous)
		{
		}
	}
}
