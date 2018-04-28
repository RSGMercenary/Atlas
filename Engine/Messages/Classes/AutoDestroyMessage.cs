using Atlas.Engine.Engine;

namespace Atlas.Engine.Messages
{
	class AutoDestroyMessage : PropertyMessage<IAutoDestroyEngineObject, bool>, IAutoDestroyMessage
	{
		public AutoDestroyMessage(bool current, bool previous) : base(current, previous)
		{
		}
	}
}
