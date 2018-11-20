using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateStateMessage : PropertyMessage<IUpdate, TimeStep>, IUpdateStateMessage
	{
		public UpdateStateMessage(IUpdate messenger, TimeStep current, TimeStep previous) : base(messenger, current, previous)
		{
		}
	}
}
