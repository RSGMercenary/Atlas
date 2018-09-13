using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	public interface IManagerRemoveMessage : IKeyValueMessage<IComponent, int, IEntity>
	{

	}
}
