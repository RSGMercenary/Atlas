using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface IUpdateState : IMessenger
	{
		TimeStep UpdateState { get; }
	}
}
