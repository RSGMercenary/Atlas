using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	public interface IManagerRemoveMessage<out T> : IKeyValueMessage<T, int, IEntity>
		where T : IComponent
	{

	}
}