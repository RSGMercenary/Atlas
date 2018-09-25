using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Messages
{
	public interface IEngineMessage<out T> : IPropertyMessage<T, IEngine>
		where T : IObject
	{
	}
}
