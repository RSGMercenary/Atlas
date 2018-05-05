using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	public interface IManagerRemoveMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
