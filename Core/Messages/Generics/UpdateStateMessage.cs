using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateStateMessage<TMessenger> : PropertyMessage<TMessenger, TimeStep>, IUpdateStateMessage<TMessenger>
		where TMessenger : IUpdateState
	{
		public UpdateStateMessage(TMessenger messenger, TimeStep current, TimeStep previous) : base(messenger, current, previous)
		{
		}
	}
}
