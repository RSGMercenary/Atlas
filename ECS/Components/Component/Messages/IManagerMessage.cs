using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Messages
{
	public interface IManagerMessage<out T> : IMessage<T>
		where T : IComponent
	{

	}
}