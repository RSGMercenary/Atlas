using Atlas.Core.Collections.LinkList;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Extensions;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Components.Engine.Updates;
using Atlas.ECS.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Atlas.ECS.Families;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class AtlasFamily<TFamilyMember> : IFamily<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
{
	#region Events
	public event Action<IReadOnlyFamily, IEngine, IEngine> EngineChanged
	{
		add => EngineManager.EngineChanged += value;
		remove => EngineManager.EngineChanged -= value;
	}
	public event Action<IReadOnlyFamily<TFamilyMember>, TFamilyMember> MemberAdded;
	public event Action<IReadOnlyFamily<TFamilyMember>, TFamilyMember> MemberRemoved;
	#endregion

	#region Fields
	private readonly EngineManager<IReadOnlyFamily> EngineManager;

	//Reflection Fields
	private readonly FieldInfo entityField;
	private readonly Dictionary<Type, FieldInfo> componentFields = new();

	//Family Members
	private readonly LinkList<TFamilyMember> members = new();
	private readonly Dictionary<IEntity, TFamilyMember> entities = new();

	//Pooling
	private readonly List<TFamilyMember> removed = new();
	private readonly List<TFamilyMember> added = new();
	#endregion

	#region Construct / Dispose
	public AtlasFamily()
	{
		EngineManager = new(this, EngineChanging);

		var type = typeof(TFamilyMember);
		var flags = BindingFlags.NonPublic | BindingFlags.Instance;

		entityField = type.FindField<IEntity>(flags);

		foreach(var field in type.FindFields<IComponent>(flags))
			componentFields.Add(field.FieldType, field);

		PoolManager.Instance.AddPool<TFamilyMember>();
	}

	public void Dispose()
	{
		//Can't dispose Family mid-update.
		if(Engine != null || removed.Count > 0)
			return;
		Disposing();
	}

	protected virtual void Disposing()
	{
		PoolManager.Instance.RemovePool<TFamilyMember>();
	}
	#endregion

	#region Engine
	public IEngine Engine
	{
		get => EngineManager.Engine;
		set => EngineManager.Engine = value;
	}

	private void EngineChanging(IEngine current, IEngine previous)
	{
		if(current == null)
			Dispose();
	}
	#endregion

	#region Get
	public IEnumerator<TFamilyMember> GetEnumerator() => members.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	IReadOnlyLinkList<IFamilyMember> IReadOnlyFamily.Members => members;

	[JsonIgnore]
	public IReadOnlyLinkList<TFamilyMember> Members => members;

	[JsonProperty(PropertyName = nameof(Members))]
	[ExcludeFromCodeCoverage]
	private IEnumerable<TFamilyMember> SerializeMembers
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

		var member = PoolManager.Instance.Get<TFamilyMember>();
		SetMemberValues(member, entity);
		entities.Add(entity, member);

		if(!IsUpdating)
			AddMember(member);
		else
		{
			added.Add(member);
			Engine.Updates.IsUpdatingChanged -= UpdateMembers;
			Engine.Updates.IsUpdatingChanged += UpdateMembers;
		}

		//TO-DO Should this only be done after the update?
		MemberAdded?.Invoke(this, member);
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
		if(!entities.TryGetValue(entity, out var member))
			return;
		entities.Remove(entity);
		members.Remove(member);
		added.Remove(member);

		//TO-DO Should this only be done after the update?
		MemberRemoved?.Invoke(this, member);

		if(!IsUpdating)
			RemoveMember(member);
		else
		{
			removed.Add(member);
			Engine.Updates.IsUpdatingChanged -= UpdateMembers;
			Engine.Updates.IsUpdatingChanged += UpdateMembers;
		}
	}

	private void RemoveMember(TFamilyMember member)
	{
		SetMemberValues(member, null);
		PoolManager.Instance.Put(member);
	}
	#endregion

	#region Helpers
	private void UpdateMembers(IUpdateManager manager, bool isUpdating)
	{
		if(isUpdating)
			return;
		manager.IsUpdatingChanged -= UpdateMembers;
		while(removed.Count > 0)
			RemoveMember(removed.Pop());
		while(added.Count > 0)
			AddMember(added.Pop());
		if(Engine == null)
			Dispose();
	}

	private bool IsUpdating => Engine?.Updates.IsUpdating ?? false;

	private void SetMemberValues(TFamilyMember member, IEntity entity)
	{
		entityField.SetValue(member, entity);
		foreach(var type in componentFields.Keys)
		{
			var component = entity != null ? entity.GetComponent(type) : null;
			componentFields[type].SetValue(member, component);
		}
	}

	public void SortMembers(Action<ILinkList<TFamilyMember>, Func<TFamilyMember, TFamilyMember, int>> sorter, Func<TFamilyMember, TFamilyMember, int> compare)
	{
		sorter.Invoke(members, compare);
	}
	#endregion
}