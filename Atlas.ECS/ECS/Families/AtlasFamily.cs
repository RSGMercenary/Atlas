using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Extensions;
using Atlas.Core.Messages;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Atlas.ECS.Families;

[JsonObject]
public class AtlasFamily<TFamilyMember> : Messenger<IReadOnlyFamily<TFamilyMember>>, IFamily<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
{
	#region Fields
	private readonly EngineItem<IReadOnlyFamily<TFamilyMember>> EngineItem;

	//Reflection Fields
	private readonly FieldInfo entityField;
	private readonly Dictionary<Type, FieldInfo> componentFields = new();

	//Family Members
	private readonly Group<TFamilyMember> members = new();
	private readonly Dictionary<IEntity, TFamilyMember> entities = new();

	//Pooling
	private readonly List<TFamilyMember> added = new();
	private readonly List<TFamilyMember> removed = new();
	private readonly Pool<TFamilyMember> pool = new InstancePool<TFamilyMember>();
	#endregion

	#region Construct / Dispose
	public AtlasFamily()
	{
		EngineItem = new(this, EngineChanged);

		var type = typeof(TFamilyMember);
		var flags = BindingFlags.NonPublic | BindingFlags.Instance;

		entityField = type.FindField<IEntity>(flags);

		foreach(var field in type.FindFields<IComponent>(flags))
			componentFields.Add(field.FieldType, field);
	}

	public sealed override void Dispose()
	{
		//Can't dispose Family mid-update.
		if(Engine != null || removed.Count > 0)
			return;
		base.Dispose();
	}
	#endregion

	#region Engine
	public IEngine Engine
	{
		get => EngineItem.Engine;
		set => EngineItem.Engine = value;
	}

	private void EngineChanged(IEngine current, IEngine previous)
	{
		if(current == null)
			Dispose();
	}
	#endregion

	#region Get
	public IEnumerator<TFamilyMember> GetEnumerator() => members.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	IReadOnlyGroup<IFamilyMember> IReadOnlyFamily.Members => Members;

	[JsonIgnore]
	public IReadOnlyGroup<TFamilyMember> Members => members;

	[JsonProperty(PropertyName = nameof(Members))]
	[ExcludeFromCodeCoverage]
	private IEnumerable<TFamilyMember> JsonPropertyMembers
	{
		get => Members;
		set
		{
			foreach(var member in value)
				AddMember(member);
		}
	}

	IFamilyMember IReadOnlyFamily.GetMember(IEntity entity) => GetMember(entity);

	public TFamilyMember GetMember(IEntity entity) => entities.ContainsKey(entity) ? entities[entity] : null;
	#endregion

	#region Has
	public bool HasMember(IEntity entity) => entities.ContainsKey(@entity);
	#endregion

	#region Add
	public void AddEntity(IEntity entity) => Add(entity);

	public void AddEntity(IEntity entity, Type componentType)
	{
		if(componentFields.ContainsKey(componentType))
			Add(entity);
	}

	private void Add(IEntity entity)
	{
		if(entities.ContainsKey(entity))
			return;
		foreach(var type in componentFields.Keys)
		{
			if(!entity.HasComponent(type))
				return;
		}

		var member = SetMemberValues(pool.Get(), entity, true);
		entities.Add(entity, member);

		if(!IsUpdating)
			AddMember(member);
		else
		{
			added.Add(member);
			Engine.AddListener<IUpdateStateMessage<IEngine>>(UpdateMembers);
		}

		Message<IFamilyMemberAddMessage<TFamilyMember>>(new FamilyMemberAddMessage<TFamilyMember>(member));
	}

	private void AddMember(TFamilyMember member) => members.Add(member);
	#endregion

	#region Remove
	public void RemoveEntity(IEntity entity) => Remove(entity);

	public void RemoveEntity(IEntity entity, Type componentType)
	{
		if(componentFields.ContainsKey(componentType))
			Remove(entity);
	}

	private void Remove(IEntity entity)
	{
		if(!entities.ContainsKey(entity))
			return;
		var member = entities[entity];
		entities.Remove(entity);
		members.Remove(member);
		added.Remove(member);
		Message<IFamilyMemberRemoveMessage<TFamilyMember>>(new FamilyMemberRemoveMessage<TFamilyMember>(member));

		if(!IsUpdating)
			RemoveMember(member);
		else
		{
			removed.Add(member);
			Engine.AddListener<IUpdateStateMessage<IEngine>>(UpdateMembers);
		}
	}

	private void RemoveMember(TFamilyMember member)
	{
		pool.Release(SetMemberValues(member, null, false));
	}
	#endregion

	#region Helpers
	private void UpdateMembers(IUpdateStateMessage<IEngine> message)
	{
		if(message.CurrentValue != TimeStep.None)
			return;
		message.Messenger.RemoveListener<IUpdateStateMessage<IEngine>>(UpdateMembers);
		while(removed.Count > 0)
			RemoveMember(removed.Pop());
		while(added.Count > 0)
			AddMember(added.Pop());
		if(Engine == null)
			Dispose();
	}

	private bool IsUpdating => (Engine?.UpdateState ?? TimeStep.None) != TimeStep.None;

	private TFamilyMember SetMemberValues(TFamilyMember member, IEntity entity, bool add)
	{
		entityField.SetValue(member, entity);
		foreach(var type in componentFields.Keys)
			componentFields[type].SetValue(member, add ? entity.GetComponent(type) : null);
		return member;
	}

	public void SortMembers(Action<IList<TFamilyMember>, Func<TFamilyMember, TFamilyMember, int>> sorter, Func<TFamilyMember, TFamilyMember, int> compare)
	{
		sorter.Invoke(members, compare);
	}
	#endregion
}