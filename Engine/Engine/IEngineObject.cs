namespace Atlas.Engine.Engine
{
	interface IEngineObject<T>:IReadOnlyEngineObject<T>
	{
		new IEngine Engine { get; set; }
	}
}
