using Atlas.ECS.Components;
using Atlas.ECS.Objects;

namespace Atlas.Framework.Messages
{
	class EngineMessage : PropertyMessage<IEngineObject, IEngine>, IEngineMessage
	{
		public EngineMessage()
		{

		}

		public EngineMessage(IEngine current, IEngine previous) : base(current, previous)
		{
		}
	}
}
