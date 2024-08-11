using Atlas.ECS.Entities;
using Newtonsoft.Json;

namespace Atlas.ECS.Families;

[JsonObject]
public class AtlasFamilyMember : IFamilyMember
{
	public IEntity Entity { get; }
}