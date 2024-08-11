using Atlas.ECS.Entities;
using Newtonsoft.Json;

namespace Atlas.ECS.Serialization;

public static class AtlasSerializer
{
	//DO NOT MAKE THIS A METHOD!!
	//Newtonsoft does some weird caching when it's a method.
	//Breakpoints then won't be hit when debugging.
	private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
	{
		ContractResolver = new AtlasContractResolver(),
		NullValueHandling = NullValueHandling.Ignore,
		ObjectCreationHandling = ObjectCreationHandling.Replace,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Objects,
	};

	public static string Serialize(IEntity value, Formatting formatting, int maxDepth)
	{
		(Settings.ContractResolver as AtlasContractResolver).MaxDepth = maxDepth;
		return Serialize(value, formatting);
	}

	public static string Serialize<T>(T value, Formatting formatting)
		where T : ISerialize
	{
		(Settings.ContractResolver as AtlasContractResolver).Instance = value;
		return JsonConvert.SerializeObject(value, formatting, Settings);
	}

	public static T Deserialize<T>(string json)
		where T : ISerialize
	{
		return JsonConvert.DeserializeObject<T>(json, Settings);
	}
}