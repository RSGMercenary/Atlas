using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Atlas.ECS.Serialization.ContractResolvers;

internal abstract class AtlasContractResolver : DefaultContractResolver
{
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

	protected virtual bool ShouldSerialize(JsonProperty property, object value, Predicate<object> shouldSerialize)
	{
		return shouldSerialize?.Invoke(value) ?? true;
	}

	public bool RemoveComponents(JsonProperty property, object value) => value is IEntity entity && property.PropertyName == nameof(entity.Components);

	public bool RemoveChildren(JsonProperty property, object value) => value is IEntity entity && property.PropertyName == nameof(entity.Children);

	public bool RemoveManagers(JsonProperty property, object value) => value is IComponent component && property.PropertyName == nameof(component.Managers);
}