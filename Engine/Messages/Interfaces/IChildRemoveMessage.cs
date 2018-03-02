using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	interface IChildRemoveMessage : IKeyValueMessage<IEntity, int, IEntity>
	{

	}
}
