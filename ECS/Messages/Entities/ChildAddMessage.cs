using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	class ChildAddMessage : KeyValueMessage<IEntity, int, IEntity>, IChildAddMessage
	{
		public ChildAddMessage(IEntity messenger, int key, IEntity value) : base(messenger, key, value)
		{
		}
	}
}
