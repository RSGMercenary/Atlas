using Atlas.ECS.Systems;

namespace Atlas.Core.Messages
{
	public interface IFixedTimeMessage : IPropertyMessage<IReadOnlySystem, double>
	{
	}
}
