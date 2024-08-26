using Atlas.Core.Collections.Group;
using Atlas.ECS.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Entities;

internal class EntityManager : IEntityManager
{
	private readonly Group<IEntity> list = new();
	private readonly Dictionary<string, IEntity> dictionary = new();

	public event Action<IEntityManager, IEntity> Added;
	public event Action<IEntityManager, IEntity> Removed;

	public IEngine Engine { get; }

	internal EntityManager(IEngine engine)
	{
		Engine = engine;
	}

	#region Add/Remove
	internal void AddEntity(IEntity entity)
	{
		//Change the Entity's global name if it already exists.
		if(dictionary.ContainsKey(entity.GlobalName))
			entity.GlobalName = AtlasEntity.UniqueName;

		dictionary[entity.GlobalName] = entity;
		list.Add(entity);
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
		//Protect against parents signaling a child being removed which never got to be added.
		if(!dictionary.TryGetValue(entity.GlobalName, out var global) || global != entity)
			return;

		entity.ChildAdded -= ChildAdded;
		entity.RootChanged -= RootChanged;
		entity.GlobalNameChanged -= GlobalNameChanged;

		foreach(var child in entity.Children.Backward())
			RemoveEntity(child);

		Removed?.Invoke(this, entity);

		dictionary.Remove(entity.GlobalName);
		list.Remove(entity);
		entity.Engine = null;
	}
	#endregion

	#region Get
	[JsonIgnore]
	public IReadOnlyGroup<IEntity> Entities => list;

	public IEntity Get(string globalName) => dictionary.TryGetValue(globalName, out var entity) ? entity : null;
	#endregion

	#region Has
	public bool Has(string globalName) => dictionary.ContainsKey(globalName);

	public bool Has(IEntity entity) => dictionary.TryGetValue(entity.GlobalName, out var global) && global == entity;
	#endregion

	#region Messages
	private void ChildAdded(IEntity parent, IEntity child, int index)
	{
		if(!dictionary.TryGetValue(child.GlobalName, out var entity) || entity != child)
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
		if(dictionary.TryGetValue(previous, out var global) && global == entity)
		{
			dictionary.Remove(previous);
			dictionary.Add(current, entity);
		}
	}
	#endregion
}