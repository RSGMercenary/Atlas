using Newtonsoft.Json.Serialization;
using System;

namespace Atlas.ECS.Serialization.ContractResolvers;

internal class ComponentContractResolver : AtlasContractResolver
{
	public bool IsEngine { get; set; }

	protected override bool ShouldSerialize(JsonProperty property, object value, Predicate<object> shouldSerialize)
	{
		if(RemoveChildren(property, value))
			return false;
		if(RemoveComponents(property, value))
			return false;
		if(IsEngine && RemoveManagers(property, value))
			return false;
		return base.ShouldSerialize(property, value, shouldSerialize);
	}
}