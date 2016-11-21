using Atlas.Engine;
using Atlas.Interfaces;

namespace Atlas.Systems
{
	interface ISystem:IEngine<ISystem>, IPriority<ISystem>, IUpdate<ISystem>, ISleep, IDispose
	{

	}
}
