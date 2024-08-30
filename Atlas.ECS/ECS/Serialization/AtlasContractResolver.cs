﻿using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Atlas.ECS.Serialization;

public class AtlasContractResolver : DefaultContractResolver
{
	public ISerialize Instance { get; set; }
	public int MaxDepth { get; set; }

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		var property = base.CreateProperty(member, memberSerialization);

		if(property.DeclaringType.IsAssignableTo(typeof(IFamilyMember)))
			property.PropertyName = GetFamilyMemberPropertyName(property.PropertyName);

		var shouldSerialize = property.ShouldSerialize;
		property.ShouldSerialize = value => ShouldSerialize(property, value, shouldSerialize);
		return property;
	}

	private string GetFamilyMemberPropertyName(string name)
	{
		var index1 = name.IndexOf('<') + 1;
		var index2 = name.IndexOf('>');
		return name.Substring(index1, index2 - index1);
	}

	private bool ShouldSerialize(JsonProperty property, object value, Predicate<object> shouldSerialize)
	{
		if(Instance is IEntity)
		{
			if(RemoveChildren(property, value) && IsOverMaxDepth(value as IEntity))
				return false;
			if(RemoveManagers(property, value))
				return false;
		}
		if(Instance is IComponent)
		{
			if(RemoveChildren(property, value))
				return false;
			if(RemoveComponents(property, value))
				return false;
		}
		if(Instance is ISystem)
		{
			if(RemoveChildren(property, value))
				return false;
			if(RemoveComponents(property, value))
				return false;
			if(RemoveManagers(property, value))
				return false;
		}
		if(Instance is IFamily)
		{
			if(RemoveChildren(property, value))
				return false;
			if(RemoveComponents(property, value))
				return false;
			if(RemoveManagers(property, value))
				return false;
		}
		return shouldSerialize?.Invoke(value) ?? true;
	}

	public bool RemoveComponents(JsonProperty property, object value) => value is IEntity entity && property.PropertyName == nameof(entity.Components);

	public bool RemoveChildren(JsonProperty property, object value) => value is IEntity entity && property.PropertyName == nameof(entity.Children);

	public bool RemoveManagers(JsonProperty property, object value) => value is IComponent component && property.PropertyName == nameof(component.Managers);

	private bool IsOverMaxDepth(IEntity entity) => MaxDepth < 0 ? false : GetDepth(entity) >= MaxDepth;

	private int GetDepth(IEntity entity)
	{
		var depth = 0;
		while(entity.Parent != null)
		{
			entity = entity.Parent;
			++depth;
		}
		return depth;
	}
}