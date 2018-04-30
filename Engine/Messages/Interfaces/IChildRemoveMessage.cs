using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	public interface IChildRemoveMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
