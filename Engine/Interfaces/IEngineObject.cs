using Atlas.Engine.Components;

namespace Atlas.Engine.Interfaces
{
	interface IEngineObject<T> : IReadOnlyEngineObject<T>
	{
		new IEngine Engine { get; set; }
	}
}
