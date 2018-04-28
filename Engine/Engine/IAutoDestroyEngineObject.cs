namespace Atlas.Engine.Engine
{
	public interface IAutoDestroyEngineObject : IEngineObject
	{
		/// <summary>
		/// Determines whether a given
		/// </summary>
		bool AutoDestroy { get; set; }
	}
}
