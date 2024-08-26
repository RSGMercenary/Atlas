using Atlas.Core.Collections.Group;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine.Entities;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Families;

internal class FamilyManager : IFamilyManager
{
	public event Action<IFamilyManager, IFamily> Added;
	public event Action<IFamilyManager, IFamily> Removed;

	private readonly Group<IFamily> list = new();
	private readonly Dictionary<Type, IFamily> dictionary = new();
	private readonly Dictionary<Type, int> references = new();

	public IEngine Engine { get; }

	internal FamilyManager(IEngine engine)
	{
		Engine = engine;
		Engine.Entities.Added += EntityAdded;
		Engine.Entities.Removed += EntityRemoved;
	}

	private void EntityAdded(IEntityManager manager, IEntity entity)
	{
		foreach(var family in list)
			family.AddEntity(entity);
		entity.ComponentAdded += ComponentAdded;
		entity.ComponentRemoved += ComponentRemoved;
	}

	private void EntityRemoved(IEntityManager manager, IEntity entity)
	{
		entity.ComponentAdded -= ComponentAdded;
		entity.ComponentRemoved -= ComponentRemoved;
		foreach(var family in list)
			family.RemoveEntity(entity);
	}

	#region Create
	protected virtual IFamily<TFamilyMember> CreateFamily<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new() => new AtlasFamily<TFamilyMember>();
	#endregion

	#region Add
	public IReadOnlyFamily<TFamilyMember> Add<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new()
	{
		var type = typeof(TFamilyMember);
		if(!dictionary.TryGetValue(type, out var family))
		{
			family = CreateFamily<TFamilyMember>();
			list.Add(family);
			dictionary.Add(type, family);
			references.Add(type, 0);
			family.Engine = Engine;
			foreach(var entity in Engine.Entities.Entities)
				family.AddEntity(entity);

			Added?.Invoke(this, family);
		}
		++references[type];
		return (IReadOnlyFamily<TFamilyMember>)dictionary[type];
	}
	#endregion

	#region Remove
	public void Remove<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new()
	{
		var type = typeof(TFamilyMember);
		if(!dictionary.TryGetValue(type, out var family))
			return;
		if(--references[type] > 0)
			return;
		list.Remove(family);
		dictionary.Remove(type);
		references.Remove(type);
		family.Engine = null;

		Removed?.Invoke(this, family);
	}
	#endregion

	#region Get
	[JsonIgnore]
	public IReadOnlyGroup<IReadOnlyFamily> Families => list;

	public IReadOnlyFamily<TFamilyMember> Get<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new()
	{
		return (IReadOnlyFamily<TFamilyMember>)Get(typeof(TFamilyMember));
	}

	public IReadOnlyFamily Get(Type type) => dictionary.TryGetValue(type, out var family) ? family : null;
	#endregion

	#region Has
	public bool Has(IReadOnlyFamily family) => dictionary.ContainsValue((IFamily)family);

	public bool Has<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new()
	{
		return Has(typeof(TFamilyMember));
	}

	public bool Has(Type type) => dictionary.ContainsKey(type);
	#endregion

	#region Messages
	private void ComponentAdded(IEntity entity, IComponent component, Type type)
	{
		foreach(var family in list)
			family.AddEntity(entity, type);
	}

	private void ComponentRemoved(IEntity entity, IComponent component, Type type)
	{
		foreach(var family in list)
			family.RemoveEntity(entity, type);
	}
	#endregion
}