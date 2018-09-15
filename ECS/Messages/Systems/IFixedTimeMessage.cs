using Atlas.Core.Messages;
using Atlas.ECS.Systems;

namespace Atlas.ECS.Messages
{
	public interface IFixedTimeMessage : IPropertyMessage<IReadOnlySystem, double>
	{
	}
}
