using Atlas.Engine.Interfaces;

namespace Atlas.Engine.Systems
{
	interface ISystem:IEngineObject<ISystem>, IBaseObject<ISystem>, IPriority<ISystem>, ISleep<ISystem>
	{
		void FixedUpdate(double deltaFixedUpdateTime);
		void Update(double deltaUpdateTime);
	}
}
