using Atlas.Engine.Entities;

namespace Atlas.Engine.Interfaces
{
	interface IEngineObject<T>:IReadOnlyEngineObject<T>
	{
		new IEngine Engine { get; set; }
	}
}
