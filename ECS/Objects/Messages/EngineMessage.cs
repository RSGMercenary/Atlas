using Atlas.Core.Messages;
using Atlas.ECS.Components;

namespace Atlas.ECS.Objects.Messages
{
	class EngineMessage<T> : PropertyMessage<T, IEngine>, IEngineMessage<T> where T : IObject
	{
		public EngineMessage(IEngine current, IEngine previous) : base(current, previous) { }
	}
}