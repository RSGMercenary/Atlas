using Atlas.Engine.Interfaces;
using System;

namespace Atlas.Engine.Systems
{
	interface ISystem : IEngineObject<ISystem>, IPriority<ISystem>, ISleep<ISystem>
	{
		Type Interface { get; set; }
		void FixedUpdate(double deltaFixedUpdateTime);
		void Update(double deltaUpdateTime);
	}
}
