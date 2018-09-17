using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateStateMessage<TMessenger> : PropertyMessage<TMessenger, TimeStep>, IUpdateStateMessage<TMessenger>
		where TMessenger : IUpdateStateObject
	{
		public UpdateStateMessage(TMessenger messenger, TimeStep current, TimeStep previous) : base(messenger, current, previous)
		{
		}
	}
}
