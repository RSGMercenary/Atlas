using Atlas.ECS.Families;

namespace Atlas.ECS.Systems
{
	public interface IFamilySystem<TFamilyMember> : ISystem where TFamilyMember : class, IFamilyMember, new()
	{
		IFamily<TFamilyMember> Family { get; }

		bool UpdateSleepingEntities { get; }
	}
}