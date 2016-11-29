using Atlas.Engine.Engine;
using Atlas.Engine.Interfaces;

namespace Atlas.Engine.Systems
{
	interface ISystem:IEngine<ISystem>, IPriority<ISystem>, IUpdate<ISystem>, ISleep<ISystem>, IDispose<ISystem>
	{

	}
}
