using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class UpdateStateMessage<T> : PropertyMessage<T, TimeStep>, IUpdateStateMessage<T>
		where T : class, IUpdateState
	{
		public UpdateStateMessage(TimeStep current, TimeStep previous) : base(current, previous)
		{
		}
	}
}
