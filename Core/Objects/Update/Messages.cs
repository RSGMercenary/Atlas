using Atlas.Core.Messages;

namespace Atlas.Core.Objects.Update
{
	public interface IUpdateStateMessage<out T> : IPropertyMessage<T, TimeStep> where T : IUpdateState, IMessenger { }

	class UpdateStateMessage<T> : PropertyMessage<T, TimeStep>, IUpdateStateMessage<T> where T : IUpdateState, IMessenger
	{
		public UpdateStateMessage(TimeStep current, TimeStep previous) : base(current, previous) { }
	}
}