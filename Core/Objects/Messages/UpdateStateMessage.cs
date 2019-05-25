using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateStateMessage<T> : PropertyMessage<T, TimeStep>, IUpdateStateMessage<T>
		where T : IUpdateState
	{
		public UpdateStateMessage(T messenger, TimeStep current, TimeStep previous) : base(messenger, current, previous)
		{
		}
	}
}
