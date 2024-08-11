using Newtonsoft.Json;

namespace Atlas.ECS.Serialization;

public interface ISerialize
{
	string Serialize(Formatting formatting = Formatting.None);
}