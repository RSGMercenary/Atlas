using Atlas.Engine.Builders;

namespace Atlas.Engine.Components
{
	interface IEntityBuilder : IComponent, IReadOnlyBuilder<IEntityBuilder>
	{

	}
}
