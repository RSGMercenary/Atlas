using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	public interface IManagerAddMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
