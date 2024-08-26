using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine.Entities;
using Atlas.ECS.Components.Engine.Families;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Components.Engine.Updates;

namespace Atlas.ECS.Components.Engine;

public interface IEngine : IComponent<IEngine>
{
	IEntityManager Entities { get; }

	IFamilyManager Families { get; }

	ISystemManager Systems { get; }

	IUpdateManager Updates { get; }
}