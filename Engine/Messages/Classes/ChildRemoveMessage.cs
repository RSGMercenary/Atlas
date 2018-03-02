using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class ChildRemoveMessage : KeyValueMessage<IEntity, int, IEntity>, IChildRemoveMessage
	{
		public ChildRemoveMessage(int key, IEntity value) : base(key, value)
		{
		}
	}
}
