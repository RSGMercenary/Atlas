using Atlas.Core.Messages;
using Atlas.ECS.Components;

namespace Atlas.ECS.Objects.Messages
{
	public interface IEngineMessage<out T> : IPropertyMessage<T, IEngine> where T : IObject { }
}