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
	private static readonly EntityContractResolver EntityContractResolver = new();
	private static readonly ComponentContractResolver ComponentContractResolver = new();
	private static readonly FamilyContractResolver FamilyContractResolver = new();
	private static readonly SystemContractResolver SystemContractResolver = new();

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

	public static string Serialize(this IEntity entity, Formatting formatting = Formatting.None, int maxDepth = -1)
	{
		EntityContractResolver.MaxDepth = maxDepth;
		Settings.ContractResolver = EntityContractResolver;
		return Serialize((ISerialize)entity, formatting);
	}

	public static string Serialize(this IComponent component, Formatting formatting = Formatting.None)
	{
		ComponentContractResolver.IsEngine = IsEngine(component);
		Settings.ContractResolver = ComponentContractResolver;
		return Serialize((ISerialize)component, formatting);
	}

	public static string Serialize(this IReadOnlyFamily family, Formatting formatting = Formatting.None)
	{
		Settings.ContractResolver = FamilyContractResolver;
		return Serialize((ISerialize)family, formatting);
	}

	public static string Serialize(this ISystem system, Formatting formatting = Formatting.None)
	{
		Settings.ContractResolver = SystemContractResolver;
		return Serialize((ISerialize)system, formatting);
	}

	private static string Serialize(ISerialize value, Formatting formatting = Formatting.None)
	{
		return JsonConvert.SerializeObject(value, formatting, Settings);
	}

	public static T Deserialize<T>(string json) where T : ISerialize
	{
		return JsonConvert.DeserializeObject<T>(json, Settings);
	}

	private static bool IsEngine(IComponent component) => component.GetType().IsAssignableTo(typeof(IEngine));
}