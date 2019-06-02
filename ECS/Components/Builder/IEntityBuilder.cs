using Atlas.Core.Builders;

namespace Atlas.ECS.Components
{
	public interface IEntityBuilder : IComponent, IReadOnlyBuilder<IEntityBuilder>
	{

	}

	public interface IEntityBuilder<T> : IComponent, IEntityBuilder
		where T : IEntityBuilder
	{
	}
}