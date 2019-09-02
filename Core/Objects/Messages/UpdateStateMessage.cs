using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateStateMessage<T> : PropertyMessage<T, TimeStep>, IUpdateStateMessage<T>
		where T : IUpdateState, IMessenger
	{
		public UpdateStateMessage(TimeStep current, TimeStep previous) : base(current, previous) { }
	}
}