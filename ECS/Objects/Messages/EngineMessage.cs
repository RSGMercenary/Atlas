using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Messages
{
	class EngineMessage : PropertyMessage<IObject, IEngine>, IEngineMessage
	{
		public EngineMessage(IObject messenger, IEngine current, IEngine previous) : base(messenger, current, previous)
		{
		}
	}
}
