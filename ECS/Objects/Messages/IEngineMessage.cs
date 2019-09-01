using Atlas.Core.Messages;
using Atlas.ECS.Components;

namespace Atlas.ECS.Objects.Messages
{
	public interface IEngineMessage<T> : IPropertyMessage<T, IEngine>
		where T : IObject
	{
	}
}