namespace Atlas.Engine.Systems
{
	public interface IEngineUpdate : IFixedUpdate, IUpdate
	{
		UpdatePhase UpdatePhase { get; }
	}
}
