using Atlas.Core.Collections.LinkList;
using Atlas.ECS.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Entities;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal sealed class EntityManager : IEntityManager
{
	#region Events
	public event Action<IEntityManager, IEntity> Added;
	public event Action<IEntityManager, IEntity> Removed;
	#endregion

	#region Fields
	private readonly LinkList<IEntity> entities = new();
	private readonly Dictionary<string, IEntity> globalNames = new();
	#endregion

	internal EntityManager(IEngine engine)
	{
		Engine = engine;
	}

	public IEngine Engine { get; }

	public bool RenameDuplicateGlobalNames { get; set; } = true;

	#region Add / Remove
	internal void AddEntity(IEntity entity)
	{
		AssertGlobalName(entity);

		globalNames[entity.GlobalName] = entity;
		entities.Add(entity);
		entity.Engine = Engine;

		entity.ChildAdded += ChildAdded;
		entity.RootChanged += RootChanged;
		entity.GlobalNameChanged += GlobalNameChanged;

		Added?.Invoke(this, entity);

		foreach(var child in entity.Children.Forward())
			AddEntity(child);
	}

	internal void RemoveEntity(IEntity entity)
	{
		//Protect against parents removing a child that never got to be added.
		if(!globalNames.TryGetValue(entity.GlobalName, out var global) || global != entity)
			return;

		foreach(var child in entity.Children.Backward())
			RemoveEntity(child);

		entity.ChildAdded -= ChildAdded;
		entity.RootChanged -= RootChanged;
		entity.GlobalNameChanged -= GlobalNameChanged;

		globalNames.Remove(entity.GlobalName);
		entities.Remove(entity);
		entity.Engine = null;

		Removed?.Invoke(this, entity);
	}
	#endregion

	private void AssertGlobalName(IEntity entity)
	{
		if(globalNames.ContainsKey(entity.GlobalName))
		{
			if(!RenameDuplicateGlobalNames)
				throw new InvalidOperationException($"'{entity.GlobalName}' already exists as a Global Name.");
			entity.GlobalName = AtlasEntity.UniqueName;
		}
	}

	#region Get
	[JsonProperty]
	public IReadOnlyLinkList<IEntity> Entities => entities;

	public IReadOnlyDictionary<string, IEntity> GlobalNames => globalNames;

	public IEntity Get(string globalName) => globalNames.TryGetValue(globalName, out var entity) ? entity : null;
	#endregion

	#region Has
	public bool Has(string globalName) => Get(globalName) != null;

	public bool Has(IEntity entity) => globalNames.TryGetValue(entity.GlobalName, out var global) && global == entity;
	#endregion

	#region Listeners
	private void ChildAdded(IEntity parent, IEntity child, int index)
	{
		if(!globalNames.TryGetValue(child.GlobalName, out var entity) || entity != child)
			AddEntity(child);
	}

	private void RootChanged(IEntity entity, IEntity current, IEntity previous)
	{
		if(entity == Engine.Manager)
			entity.RemoveComponentType(Engine);
		else if(current == null)
			RemoveEntity(entity);
	}

	private void GlobalNameChanged(IEntity entity, string current, string previous)
	{
		if(globalNames.TryGetValue(previous, out var global) && global == entity)
		{
			globalNames.Remove(previous);
			globalNames.Add(current, entity);
		}
	}
	#endregion
}