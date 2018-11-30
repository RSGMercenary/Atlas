using Atlas.Core.Collections.Group;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components
{
	public interface ISystemManager : IComponent
	{
		bool HasSystem(Type type);
		bool HasSystem<TKey>()
			where TKey : class, ISystem, new();

		bool AddSystem(Type type);
		bool AddSystem<TKey>()
			where TKey : class, ISystem, new();

		bool RemoveSystem(Type type);
		bool RemoveSystem<TKey>()
			where TKey : class, ISystem, new();
		bool RemoveSystems();

		IReadOnlyGroup<Type> Systems { get; }
	}
}
