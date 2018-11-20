using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class FreeSleepMessage : PropertyMessage<IEntity, int>, IFreeSleepMessage
	{
		public FreeSleepMessage(IEntity messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
