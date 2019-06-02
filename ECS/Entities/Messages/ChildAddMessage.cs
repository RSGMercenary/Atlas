using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class ChildAddMessage : KeyValueMessage<IEntity, int, IEntity>, IChildAddMessage
	{
		public ChildAddMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}