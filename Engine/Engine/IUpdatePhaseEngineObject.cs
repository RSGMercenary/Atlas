namespace Atlas.Engine.Engine
{
	public interface IUpdatePhaseEngineObject : IEngineObject
	{
		UpdatePhase UpdateState { get; }
	}
}
