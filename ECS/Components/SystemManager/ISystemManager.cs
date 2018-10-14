using Atlas.Core.Collections.Group;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components
{
	public interface ISystemManager : IComponent<ISystemManager>
	{
		bool HasSystem(Type type);
		bool HasSystem<TISystem>() where TISystem : ISystem;

		bool AddSystem(Type type);
		bool AddSystem<TISystem>() where TISystem : ISystem;

		bool RemoveSystem(Type type);
		bool RemoveSystem<TISystem>() where TISystem : ISystem;
		bool RemoveSystems();

		IReadOnlyGroup<Type> Systems { get; }
	}
}
