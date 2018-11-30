using Atlas.ECS.Families;

namespace Atlas.ECS.Systems
{
	public interface IFamilySystem : ISystem
	{
		IReadOnlyFamily Family { get; }
		bool UpdateSleepingEntities { get; }
	}

	public interface IFamilySystem<TFamilyMember> : IFamilySystem
		where TFamilyMember : class, IFamilyMember, new()
	{
		new IReadOnlyFamily<TFamilyMember> Family { get; }
	}
}
