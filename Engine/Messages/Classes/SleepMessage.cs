using Atlas.Engine.Engine;

namespace Atlas.Engine.Messages
{
	class SleepMessage : PropertyMessage<ISleepEngineObject, int>, ISleepMessage
	{
		public SleepMessage(int current, int previous) : base(current, previous)
		{
		}
	}
}
