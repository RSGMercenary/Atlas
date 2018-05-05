using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	public interface IManagerAddMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
