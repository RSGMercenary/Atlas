using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface IUpdateState : IMessenger
	{
		TimeStep UpdateState { get; }
	}

	public interface IUpdateState<T> : IUpdateState, IMessenger<T>
		where T : IUpdateState
	{
	}
}
