using Atlas.Engine.Engine;
using Atlas.Interfaces;

namespace Atlas.Engine.Systems
{
	interface ISystem:IEngine<ISystem>, IPriority<ISystem>, IUpdate<ISystem>, ISleep, IDispose
	{

	}
}
