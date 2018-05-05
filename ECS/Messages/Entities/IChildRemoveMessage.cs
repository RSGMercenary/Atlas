using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	public interface IChildRemoveMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
