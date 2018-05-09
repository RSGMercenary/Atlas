namespace Atlas.ECS.Systems
{
	public interface IFamilySystem : IReadOnlySystem
	{
		bool UpdateSleepingEntities { get; }
	}
}
