using Atlas.Interfaces;
using Atlas.Signals;

namespace Atlas.Systems
{
	interface ISystem:IPriority<ISystem>, IUpdate, ISleep, IDispose
	{
		ISystemManager SystemManager { get; set; }
		Signal<ISystem, ISystemManager, ISystemManager> SystemManagerChanged { get; }

		//new int ReferenceCount { get; set; }
	}
}
