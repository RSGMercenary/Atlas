namespace Atlas.Engine
{
	public interface IAutoEngineObject : IEngineObject
	{
		bool AutoDestroy { get; set; }
	}
}
