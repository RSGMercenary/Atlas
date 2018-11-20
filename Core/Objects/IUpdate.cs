using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface IUpdate : IMessenger
	{
		void Update(double deltaTime);
		TimeStep UpdateState { get; }
	}
}
