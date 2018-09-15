using Atlas.ECS.Families;

namespace Atlas.ECS.Systems
{
	public interface IFamilySystem : IReadOnlySystem
	{
		IReadOnlyFamily Family { get; }
		bool UpdateSleepingEntities { get; }
	}

	public interface IFamilySystem<TFamilyMember> : IFamilySystem
		where TFamilyMember : IFamilyMember, new()
	{
		new IReadOnlyFamily<TFamilyMember> Family { get; }
	}
}
