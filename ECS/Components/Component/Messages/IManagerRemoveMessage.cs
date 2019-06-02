using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	public interface IManagerRemoveMessage<T> : IKeyValueMessage<T, int, IEntity>
		where T : IComponent
	{

	}
}