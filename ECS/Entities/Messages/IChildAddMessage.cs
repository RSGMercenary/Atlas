using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	public interface IChildAddMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
