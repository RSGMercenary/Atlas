using Atlas.ECS.Families;

namespace Atlas.ECS.Systems
{
	public interface IFamilySystem : ISystem
	{
		IReadOnlyFamily Family { get; }
		bool UpdateSleepingEntities { get; }
	}

	public interface IFamilySystem<T> : IFamilySystem, ISystem<T>
		where T : IFamilySystem
	{

	}

	public interface IFamilySystem<T, TFamilyMember> : IFamilySystem<T>
		where T : IFamilySystem
		where TFamilyMember : IFamilyMember, new()
	{
		new IReadOnlyFamily<TFamilyMember> Family { get; }
	}
}
