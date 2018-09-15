using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Messages
{
	class EngineMessage : PropertyMessage<IEngineObject, IEngine>, IEngineMessage
	{
		public EngineMessage(IEngineObject messenger, IEngine current, IEngine previous) : base(messenger, current, previous)
		{
		}
	}
}
