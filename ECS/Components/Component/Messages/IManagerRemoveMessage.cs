using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Messages
{
	public interface IManagerRemoveMessage : IKeyValueMessage<IComponent, int, IEntity>
	{

	}
}
