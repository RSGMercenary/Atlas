using Atlas.Engine.Components;

namespace Atlas.Engine.Messages
{
	public class EngineMessage : PropertyMessage<IEngineObject, IEngine>, IEngineMessage
	{
		public EngineMessage(IEngine current, IEngine previous) : base(current, previous)
		{
		}
	}
}
