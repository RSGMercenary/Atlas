using Atlas.ECS.Entities;

namespace Atlas.ECS.Families;

public interface IFamilyMember
{
	IEntity Entity { get; }
}