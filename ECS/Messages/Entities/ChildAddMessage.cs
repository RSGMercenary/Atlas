using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class ChildAddMessage : KeyValueMessage<IEntity, int, IEntity>, IChildAddMessage
	{
		public ChildAddMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
