using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	class FreeSleepMessage : PropertyMessage<IEntity, int>, IFreeSleepMessage
	{
		public FreeSleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
