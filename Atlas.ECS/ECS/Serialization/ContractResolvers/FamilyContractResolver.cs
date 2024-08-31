using Newtonsoft.Json.Serialization;
using System;

namespace Atlas.ECS.Serialization.ContractResolvers;

internal class FamilyContractResolver : AtlasContractResolver
{
	protected override bool ShouldSerialize(JsonProperty property, object value, Predicate<object> shouldSerialize)
	{
		if(RemoveChildren(property, value))
			return false;
		if(RemoveComponents(property, value))
			return false;
		if(RemoveManagers(property, value))
			return false;
		return base.ShouldSerialize(property, value, shouldSerialize);
	}
}