using Atlas.ECS.Objects;
using Atlas.Framework.Objects;

namespace Atlas.Framework.Messages
{
	class SleepMessage : PropertyMessage<ISleepObject, int>, ISleepMessage
	{
		public SleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
