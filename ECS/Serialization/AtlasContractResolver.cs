using Atlas.ECS.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Atlas.ECS.Serialize;

public class AtlasContractResolver : DefaultContractResolver
{
	private int MaxDepth { get; set; }

	public AtlasContractResolver(int maxDepth)
	{
		MaxDepth = maxDepth;
	}

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		var property = base.CreateProperty(member, memberSerialization);
		var shouldSerialize = property.ShouldSerialize;
		property.ShouldSerialize = instance => ShouldSerialize(instance, property, shouldSerialize);
		return property;
	}

	private bool ShouldSerialize(object instance, JsonProperty property, Predicate<object> shouldSerialize)
	{
		if(MaxDepth > -1 && instance is IEntity entity && property.PropertyName == nameof(entity.Children))
		{
			var depth = 0;
			while(entity.Parent != null)
			{
				entity = entity.Parent;
				depth++;
			}
			if(depth >= MaxDepth)
				return false;
		}
		return shouldSerialize?.Invoke(instance) ?? true;
	}
}