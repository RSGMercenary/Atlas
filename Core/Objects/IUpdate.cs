using Atlas.Core.Messages;

namespace Atlas.Core.Objects
{
	public interface IUpdate : IMessenger
	{
		void Update(float deltaTime);
		TimeStep UpdateState { get; }
	}
}
