using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Messages
{
	public interface IManagerMessage<T> : IMessage<T>
		where T : IComponent
	{

	}
}