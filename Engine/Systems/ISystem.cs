using Atlas.Engine.Engine;
using Atlas.Engine.Interfaces;

namespace Atlas.Engine.Systems
{
	interface ISystem:IEngineObject<ISystem>, IPriority<ISystem>, IUpdate<ISystem>, ISleep<ISystem>
	{

	}
}
