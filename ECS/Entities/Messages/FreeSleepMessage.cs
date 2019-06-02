using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	class FreeSleepMessage : PropertyMessage<IEntity, int>, IFreeSleepMessage
	{
		public FreeSleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}