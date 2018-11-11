using Atlas.Core.Messages;
using Atlas.ECS.Systems;

namespace Atlas.ECS.Messages
{
	public interface IIntervalMessage : IPropertyMessage<ISystem, double>
	{
	}
}
