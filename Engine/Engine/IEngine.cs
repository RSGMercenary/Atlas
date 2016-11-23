namespace Atlas.Engine.Engine
{
	interface IEngine<T>:IReadOnlyEngine<T>
	{
		/// <summary>
		/// The Engine managing this instance.
		/// </summary>
		new IEngineManager Engine { get; set; }
	}
}
