using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace Atlas.ECS.Serialization.ContractResolvers;

internal class EntityContractResolver : AtlasContractResolver
{
	public int MaxDepth { get; set; }
	public string[] Properties { get; set; }

	protected override bool ShouldSerialize(JsonProperty property, object value, Predicate<object> shouldSerialize)
	{
		if(RemoveChildren(property, value) && IsOverMaxDepth(value as IEntity))
			return false;
		if(RemoveManagers(property, value))
			return false;
		if(property.DeclaringType.IsAssignableTo(typeof(IEngine)))
			return false;
		if(Properties?.Length > 0 && property.PropertyName != nameof(IEntity.Children) &&
			!Properties.Any(property.PropertyName.Equals) && property.DeclaringType.IsAssignableTo(typeof(IEntity)))
			return false;
		return base.ShouldSerialize(property, value, shouldSerialize);
	}

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