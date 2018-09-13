using Atlas.ECS.Components;
using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	public interface IManagerRemoveMessage : IKeyValueMessage<IComponent, int, IEntity>
	{

	}
}
