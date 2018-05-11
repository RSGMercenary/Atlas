using Atlas.ECS.Systems;

namespace Atlas.Framework.Messages
{
	public interface IFixedTimeMessage : IPropertyMessage<IReadOnlySystem, double>
	{
	}
}
