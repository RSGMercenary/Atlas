using Atlas.Core.Messages;

namespace Atlas.ECS.Systems.Messages
{
	public interface IIntervalMessage : IPropertyMessage<ISystem, double> { }
}