using Atlas.Engine.Engine;

namespace Atlas.Engine.Systems
{
	public interface IFamilySystem : ISystem
	{
		UpdatePhase UpdateMode { get; }
		bool UpdateSleepingEntities { get; }
	}
}
