using Atlas.Components;
using Atlas.Interfaces;
using System;

namespace Atlas.Systems
{
	interface ISystemManager:IComponent, IUpdate
	{
		bool HasSystem(ISystem system);
		bool HasSystem(Type type);
		bool HasSystem<T>() where T : ISystem;

		ISystem GetSystem(Type type);
		T GetSystem<T>() where T : ISystem;
	}
}
