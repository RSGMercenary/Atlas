using Atlas.Core.Collections.Group;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.SystemProvider;

public interface ISystemProvider : IComponent<ISystemProvider>
{
	event Action<ISystemProvider, Type> SystemAdded;
	event Action<ISystemProvider, Type> SystemRemoved;

	bool HasSystem(Type type);
	bool HasSystem<TKey>()
		where TKey : class, ISystem;

	bool AddSystem(Type type);
	bool AddSystem<TKey>()
		where TKey : class, ISystem;

	bool RemoveSystem(Type type);
	bool RemoveSystem<TKey>()
		where TKey : class, ISystem;
	bool RemoveSystems();

	IReadOnlyGroup<Type> Systems { get; }
}