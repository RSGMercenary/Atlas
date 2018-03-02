using Atlas.Engine.Interfaces;

namespace Atlas.Engine.Messages
{
	class SleepMessage : PropertyMessage<ISleep, int>, ISleepMessage
	{
		public SleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
