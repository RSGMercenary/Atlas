namespace Atlas.Engine
{
	interface IAutoEngineObject<T> : IEngineObject<T>
	{
		bool AutoDestroy { get; set; }
	}
}
