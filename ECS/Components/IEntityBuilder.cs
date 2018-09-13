using Atlas.Core.Builders;

namespace Atlas.ECS.Components
{
	public interface IEntityBuilder : IComponent, IReadOnlyBuilder<IEntityBuilder>
	{

	}
}
