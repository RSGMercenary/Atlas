using Atlas.ECS.Entities;
using Newtonsoft.Json;

namespace Atlas.ECS.Serialize;

public static class AtlasSerializer
{
	private static JsonSerializerSettings Settings => new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		ObjectCreationHandling = ObjectCreationHandling.Replace,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Objects,
	};

	public static string Serialize(IEntity entity, Formatting formatting, int maxDepth = -1)
	{
		var settings = Settings;
		settings.ContractResolver = new AtlasContractResolver(maxDepth);
		return JsonConvert.SerializeObject(entity, formatting, Settings);
	}

	public static IEntity Deserialize(string json)
	{
		return JsonConvert.DeserializeObject<IEntity>(json, Settings);
	}
}