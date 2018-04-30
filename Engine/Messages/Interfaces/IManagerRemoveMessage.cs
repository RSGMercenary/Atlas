using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	public interface IManagerRemoveMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
