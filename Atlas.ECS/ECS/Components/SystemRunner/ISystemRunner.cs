using Atlas.ECS.Components.Component;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.SystemRunner;

public interface ISystemRunner : IComponent<ISystemRunner>
{
	event Action<ISystemRunner, Type> Added;
	event Action<ISystemRunner, Type> Removed;

	bool HasSystem(Type type);

	bool HasSystem<TSystem>()
		where TSystem : class, ISystem;

	bool AddSystem(Type type);

	bool AddSystem<TSystem>()
		where TSystem : class, ISystem;

	bool RemoveSystem(Type type);

	bool RemoveSystem<TSystem>()
		where TSystem : class, ISystem;

	bool RemoveSystems();

	IReadOnlySet<Type> Systems { get; }
}