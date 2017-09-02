namespace Atlas.Engine
{
	public interface IAutoEngineObject<T> : IEngineObject<T>
		where T : IAutoEngineObject<T>
	{
		bool AutoDestroy { get; set; }
	}
}
