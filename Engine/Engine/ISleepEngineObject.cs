namespace Atlas.Engine.Engine
{
	public interface ISleepEngineObject : IEngineObject
	{
		bool IsSleeping { get; }
		int Sleeping { get; set; }
	}
}
