using Atlas.ECS.Entities;

namespace Atlas.Core.Messages
{
	class ChildAddMessage : KeyValueMessage<IEntity, int, IEntity>, IChildAddMessage
	{
		public ChildAddMessage(IEntity messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
