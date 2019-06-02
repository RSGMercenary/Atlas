using Atlas.ECS.Families;

namespace Atlas.ECS.Systems
{
	public interface IFamilySystem<out TFamilyMember> : ISystem
		where TFamilyMember : class, IFamilyMember, new()
	{
		IFamily<TFamilyMember> Family { get; }

		bool UpdateSleepingEntities { get; }
	}
}