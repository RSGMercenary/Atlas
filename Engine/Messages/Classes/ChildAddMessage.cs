using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class ChildAddMessage : KeyValueMessage<IEntity, int, IEntity>, IChildAddMessage
	{
		public ChildAddMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
