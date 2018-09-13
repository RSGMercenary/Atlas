using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	public interface IChildRemoveMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
