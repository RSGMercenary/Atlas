using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Messages
{
	class EngineMessage<T> : PropertyMessage<T, IEngine>, IEngineMessage<T>
		where T : IEngineObject
	{
		public EngineMessage(T messenger, IEngine current, IEngine previous) : base(messenger, current, previous)
		{
		}
	}
}
