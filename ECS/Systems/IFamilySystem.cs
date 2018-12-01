using Atlas.ECS.Families;

namespace Atlas.ECS.Systems
{
	public interface IFamilySystem : ISystem
	{
		IFamily Family { get; }
		bool UpdateSleepingEntities { get; }
	}

	public interface IFamilySystem<TFamilyMember> : IFamilySystem
		where TFamilyMember : class, IFamilyMember, new()
	{
		new IFamily<TFamilyMember> Family { get; }
	}
}
