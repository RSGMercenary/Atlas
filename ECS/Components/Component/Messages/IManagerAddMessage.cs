using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	public interface IManagerAddMessage : IKeyValueMessage<IComponent, int, IEntity>
	{

	}
}
