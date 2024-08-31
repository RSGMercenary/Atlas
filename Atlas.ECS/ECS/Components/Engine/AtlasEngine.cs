using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine.Entities;
using Atlas.ECS.Components.Engine.Families;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Components.Engine.Updates;
using Atlas.ECS.Entities;
using Newtonsoft.Json;
using System;

namespace Atlas.ECS.Components.Engine;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public sealed class AtlasEngine : AtlasComponent<IEngine>, IEngine
{
	public AtlasEngine()
	{
		Entities = new EntityManager(this);
		Families = new FamilyManager(this);
		Systems = new SystemManager(this);
		Updates = new UpdateManager(this);
	}

	[JsonProperty]
	public IEntityManager Entities { get; }

	[JsonProperty]
	public IFamilyManager Families { get; }

	[JsonProperty]
	public ISystemManager Systems { get; }

	[JsonProperty]
	public IUpdateManager Updates { get; }

	protected override void AddingManager(IEntity entity, int index)
	{
		if(!entity.IsRoot)
			throw new InvalidOperationException($"{nameof(IEngine)} can't be added to {nameof(IEntity)} when {nameof(IEntity.IsRoot)} is false.");

		base.AddingManager(entity, index);
		((EntityManager)Entities).AddEntity(entity);
	}

	protected override void RemovingManager(IEntity entity, int index)
	{
		((EntityManager)Entities).RemoveEntity(entity);
		base.RemovingManager(entity, index);
	}
}