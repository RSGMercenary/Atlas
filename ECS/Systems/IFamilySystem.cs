using Atlas.ECS.Families;

namespace Atlas.ECS.Systems
{
	public interface IReadOnlyFamilySystem : IReadOnlySystem
	{
		IReadOnlyFamily Family { get; }
		bool UpdateSleepingEntities { get; }
	}

	public interface IFamilySystem : IReadOnlyFamilySystem, ISystem
	{
		new bool UpdateSleepingEntities { get; set; }
	}

	public interface IReadOnlyFamilySystem<T> : IReadOnlyFamilySystem, IReadOnlySystem<T>
		where T : IReadOnlyFamilySystem
	{

	}

	public interface IFamilySystem<T> : IFamilySystem, ISystem<T>
		where T : IFamilySystem
	{

	}

	public interface IReadOnlyFamilySystem<T, TFamilyMember> : IReadOnlyFamilySystem<T>
		where T : IReadOnlyFamilySystem
		where TFamilyMember : IFamilyMember, new()
	{
		new IReadOnlyFamily<TFamilyMember> Family { get; }
	}

	public interface IFamilySystem<T, TFamilyMember> : IFamilySystem<T>
		where T : IFamilySystem
		where TFamilyMember : IFamilyMember, new()
	{

	}
}
