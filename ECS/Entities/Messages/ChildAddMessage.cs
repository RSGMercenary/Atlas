using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ChildAddMessage : KeyValueMessage<IEntity, int, IEntity>, IChildAddMessage
	{
		public ChildAddMessage(IEntity messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
