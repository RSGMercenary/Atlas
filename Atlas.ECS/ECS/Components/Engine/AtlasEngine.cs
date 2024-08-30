using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine.Entities;
using Atlas.ECS.Components.Engine.Families;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Components.Engine.Updates;
using Atlas.ECS.Entities;
using Newtonsoft.Json;
using System;

namespace Atlas.ECS.Components.Engine;

[JsonObject]
public sealed class AtlasEngine : AtlasComponent<IEngine>, IEngine
{
	private readonly EntityManager entities;
	private readonly FamilyManager families;
	private readonly SystemManager systems;
	private readonly UpdateManager updates;

	public AtlasEngine()
	{
		entities = new EntityManager(this);
		families = new FamilyManager(this);
		systems = new SystemManager(this);
		updates = new UpdateManager(this);
	}

	public IEntityManager Entities => entities;
	public IFamilyManager Families => families;
	public ISystemManager Systems => systems;
	public IUpdateManager Updates => updates;

	protected override void AddingManager(IEntity entity, int index)
	{
		if(!entity.IsRoot)
			throw new InvalidOperationException($"{nameof(IEngine)} can't be added to {nameof(IEntity)} when {nameof(IEntity.IsRoot)} is false.");

		base.AddingManager(entity, index);
		entities.AddEntity(entity);
	}

	protected override void RemovingManager(IEntity entity, int index)
	{
		entities.RemoveEntity(entity);
		base.RemovingManager(entity, index);
	}
}