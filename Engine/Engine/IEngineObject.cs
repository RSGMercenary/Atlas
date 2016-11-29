namespace Atlas.Engine.Engine
{
	interface IEngineObject<T>:IReadOnlyEngineObject<T>
	{
		/// <summary>
		/// The Engine managing this instance.
		/// </summary>
		new IEngine Engine { get; set; }
	}
}
