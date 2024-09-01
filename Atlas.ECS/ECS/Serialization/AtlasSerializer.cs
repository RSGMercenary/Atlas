using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Serialization.ContractResolvers;
using Atlas.ECS.Systems;
using Newtonsoft.Json;

namespace Atlas.ECS.Serialization;

public static class AtlasSerializer
{
	#region Contract Resolvers
	private static readonly EntityContractResolver EntityContractResolver = new();
	private static readonly ComponentContractResolver ComponentContractResolver = new();
	private static readonly FamilyContractResolver FamilyContractResolver = new();
	private static readonly SystemContractResolver SystemContractResolver = new();
	#endregion

	#region Settings
	//DO NOT MAKE THIS A METHOD!!
	//Newtonsoft does some weird caching when it's a method.
	//Breakpoints then won't be hit when debugging.
	private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
	{
		ContractResolver = EntityContractResolver,
		NullValueHandling = NullValueHandling.Ignore,
		ObjectCreationHandling = ObjectCreationHandling.Replace,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Objects,
	};
	#endregion

	#region Serialize
	public static string Serialize(this IEntity entity, Formatting formatting = Formatting.None, int maxDepth = -1, params string[] properties)
	{
		EntityContractResolver.MaxDepth = maxDepth;
		EntityContractResolver.Properties = properties;
		Settings.ContractResolver = EntityContractResolver;
		return SerializeInstance(entity, formatting);
	}

	public static string Serialize(this IComponent component, Formatting formatting = Formatting.None)
	{
		ComponentContractResolver.IsEngine = IsEngine(component);
		Settings.ContractResolver = ComponentContractResolver;
		return SerializeInstance(component, formatting);
	}

	public static string Serialize(this IReadOnlyFamily family, Formatting formatting = Formatting.None)
	{
		Settings.ContractResolver = FamilyContractResolver;
		return SerializeInstance(family, formatting);
	}

	public static string Serialize(this ISystem system, Formatting formatting = Formatting.None)
	{
		Settings.ContractResolver = SystemContractResolver;
		return SerializeInstance(system, formatting);
	}

	private static string SerializeInstance(ISerialize instance, Formatting formatting = Formatting.None)
	{
		return JsonConvert.SerializeObject(instance, formatting, Settings);
	}
	#endregion

	#region Deserialize
	public static T Deserialize<T>(string json) where T : ISerialize
	{
		return JsonConvert.DeserializeObject<T>(json, Settings);
	}
	#endregion

	#region Helpers
	private static bool IsEngine(IComponent component) => component.GetType().IsAssignableTo(typeof(IEngine));
	#endregion
}