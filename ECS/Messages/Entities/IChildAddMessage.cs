using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	public interface IChildAddMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
