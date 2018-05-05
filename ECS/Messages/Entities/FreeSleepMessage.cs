using Atlas.ECS.Entities;

namespace Atlas.Framework.Messages
{
	class FreeSleepMessage : PropertyMessage<IEntity, int>, IFreeSleepMessage
	{
		public FreeSleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
