using Atlas.Engine;
using Atlas.Interfaces;
using Atlas.Signals;

namespace Atlas.Systems
{
	interface ISystem:IPriority<ISystem>, IUpdate<ISystem>, ISleep, IDispose
	{
		IEngine Engine { get; set; }
		Signal<ISystem, IEngine, IEngine> EngineChanged { get; }
	}
}
