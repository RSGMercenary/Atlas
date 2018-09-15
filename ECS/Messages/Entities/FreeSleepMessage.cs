using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	class FreeSleepMessage : PropertyMessage<IEntity, int>, IFreeSleepMessage
	{
		public FreeSleepMessage(IEntity messenger, int current, int previous) : base(messenger, current, previous)
		{
		}
	}
}
