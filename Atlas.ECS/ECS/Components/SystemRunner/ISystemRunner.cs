using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.SystemRunner;

/// <summary>
/// An <see langword="interface"/> providing <see cref="ISystem"/> <see cref="Type"/> values to be run by <see cref="ISystemManager"/>.
/// </summary>
public interface ISystemRunner : IComponent<ISystemRunner>
{
	/// <summary>
	/// The <see langword="event"/> invoked when an <see cref="ISystem"/> <see cref="Type"/> is added.
	/// </summary>
	event Action<ISystemRunner, Type> Added;

	/// <summary>
	/// The <see langword="event"/> invoked when an <see cref="ISystem"/> <see cref="Type"/> is removed.
	/// </summary>
	event Action<ISystemRunner, Type> Removed;

	IReadOnlySet<Type> Types { get; }

	bool Add(Type type);

	bool Add<TSystem>()
		where TSystem : class, ISystem;

	bool Remove(Type type);

	bool Remove<TSystem>()
		where TSystem : class, ISystem;

	bool RemoveAll();

	bool Has(Type type);

	bool Has<TSystem>()
		where TSystem : class, ISystem;
}