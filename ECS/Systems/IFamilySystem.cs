using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public interface IFamilySystem : IReadOnlySystem
	{
		UpdatePhase UpdateMode { get; }
		bool UpdateSleepingEntities { get; }
	}
}
