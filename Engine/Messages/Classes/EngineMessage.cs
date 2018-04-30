using Atlas.Engine.Components;
using Atlas.Engine.Engine;

namespace Atlas.Engine.Messages
{
	class EngineMessage : PropertyMessage<IEngineObject, IEngine>, IEngineMessage
	{
		public EngineMessage(IEngine current, IEngine previous) : base(current, previous)
		{
		}
	}
}
