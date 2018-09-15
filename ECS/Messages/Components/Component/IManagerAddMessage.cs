using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	public interface IManagerAddMessage : IKeyValueMessage<IComponent, int, IEntity>
	{

	}
}
