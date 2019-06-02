using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Messages
{
	class EngineMessage<T> : PropertyMessage<T, IEngine>, IEngineMessage<T>
		where T : class, IObject
	{
		public EngineMessage(IEngine current, IEngine previous) : base(current, previous)
		{
		}
	}
}