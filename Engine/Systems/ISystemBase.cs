using Atlas.Engine.Engine;

namespace Atlas.Engine.Systems
{
	public interface ISystemBase : ISleepEngineObject, IUpdatePhaseEngineObject
	{
		int Priority { get; }
	}
}
