using Atlas.ECS.Families;
using System.Collections.Generic;

namespace Atlas.ECS.Systems;

public interface IFamilySystem<TFamilyMember> : ISystem, IEnumerable<TFamilyMember> where TFamilyMember : class, IFamilyMember, new()
{
	IReadOnlyFamily<TFamilyMember> Family { get; }

	bool UpdateSleepingEntities { get; }
}