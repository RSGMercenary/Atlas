using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	public interface IChildRemoveMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}