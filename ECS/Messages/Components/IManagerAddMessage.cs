using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	public interface IManagerAddMessage : IKeyValueMessage<IComponent, int, IEntity>
	{

	}
}
