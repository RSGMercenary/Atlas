using Atlas.ECS.Components.Component;
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
		var shouldSerialize = property.ShouldSerialize;
		property.ShouldSerialize = value => ShouldSerialize(property, value, shouldSerialize);
		return property;
	}

	private bool ShouldSerialize(JsonProperty property, object value, Predicate<object> shouldSerialize)
	{
		if(!(shouldSerialize?.Invoke(value) ?? true))
			return false;
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
		return true;
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
			depth++;
		}
		return depth;
	}
}