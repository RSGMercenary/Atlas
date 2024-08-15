using Atlas.ECS.Entities;
using Newtonsoft.Json;

namespace Atlas.ECS.Families;

[JsonObject(MemberSerialization = MemberSerialization.Fields)]
public class AtlasFamilyMember : IFamilyMember
{
	public IEntity Entity { get; }
}