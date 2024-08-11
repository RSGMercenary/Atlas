using Atlas.ECS.Serialization;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Atlas.ECS.Entities;

public static class EntityExtensions
{
	public static IEntityBuilder Builder(this IEntity entity)
	{
		return new AtlasEntityBuilder(entity);
	}

	public static bool IsEqual(this IEntity entity, IEntity other) => entity.Serialize() == other.Serialize();

	/// <summary>
	/// For visualization ONLY. This is not for (de)serializing! Use Serialize() instead.
	/// </summary>
	/// <param name="entity"></param>
	/// <param name="hierarchy"></param>
	/// <param name="properties"></param>
	/// <returns></returns>
	public static JToken ToJsonString(this IEntity entity, bool hierarchy, params string[] properties)
	{
		var token = JObject.Parse(entity.Serialize());
		token
			.DescendantsAndSelf()
			.OfType<JObject>()
			.Where(o => o.Value<string>("$type")?.Contains("Entity") ?? false)
			.ToList()
			.ForEach(e => SetProperties(e, hierarchy, entity.GlobalName, properties));
		return token;
	}

	private static void SetProperties(JObject entity, bool hierarchy, string globalName, params string[] properties)
	{
		SetTypeProperty(entity);
		SetChildrenProperty(entity, hierarchy, globalName);
		SetComponentProperties(entity);
		SetEntityProperties(entity, properties);
	}

	private static void SetChildrenProperty(JObject entity, bool hierarchy, string globalName)
	{
		if(!hierarchy && entity.Value<string>("GlobalName") != globalName)
			entity.Remove("Children");
	}

	private static void SetComponentProperties(JObject entity)
	{
		var components = entity.Value<JObject>("Components");
		components.Remove("$type");
		components.Remove("$id");

		foreach(var component in components.Properties().ToList())
		{
			if(component.Name.Contains("Component"))
			{
				SetTypeProperty(components[component.Name].Value<JObject>());
				component.Replace(new JProperty(SetTypeProperty(component.Name), component.Value));
			}
		}
	}

	private static void SetTypeProperty(JObject token)
	{
		var key = "$type";
		if(token.ContainsKey(key))
			token[key] = SetTypeProperty(token.Value<string>(key));
	}

	private static string SetTypeProperty(string type)
	{
		type = type.Substring(0, type.IndexOf(','));
		return type.Substring(type.LastIndexOf('.') + 1);
	}

	private static void SetEntityProperties(JObject entity, params string[] properties)
	{
		entity.Remove("$id");

		entity
			.Properties()
			.Where(p => p.Name != "Children")
			.Where(p => properties.Any() && !properties.Any(p.Name.Contains))
			.ToList()
			.ForEach(p => p.Remove());
	}
}