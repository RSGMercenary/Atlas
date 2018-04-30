using Atlas.Engine.Builders;

namespace Atlas.Engine.Components
{
	public interface IEntityBuilder : IComponent, IReadOnlyBuilder<IEntityBuilder>
	{

	}
}
