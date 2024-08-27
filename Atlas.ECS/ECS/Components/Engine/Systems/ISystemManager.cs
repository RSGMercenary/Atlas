using Atlas.Core.Collections.Group;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Systems;

public interface ISystemManager : IReadOnlyEngineManager
{
	event Action<ISystemManager, ISystem, Type> Added;

	event Action<ISystemManager, ISystem, Type> Removed;

	/// <summary>
	/// All <see cref="ISystem"/> instances being managed by the <see cref="ISystemManager"/>.
	/// <para>Systems are ordered and updated by their <see cref="ISystem.Priority"/>.</para>
	/// </summary>
	IReadOnlyGroup<ISystem> Systems { get; }

	/// <summary>
	/// All <see cref="ISystem"/> instances by <see cref="Type"/> being managed by the <see cref="ISystemManager"/>.
	/// </summary>
	IReadOnlyDictionary<Type, ISystem> Types { get; }

	ISystemCreator Creator { get; set; }

	/// <summary>
	/// Adds an <see cref="ISystem"/> with the given <see cref="Type"/> to the <see cref="ISystemManager"/>.
	/// </summary>
	/// <typeparam name="TSystem">The <see cref="ISystem"/> generic <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	TSystem Add<TSystem>()
		where TSystem : class, ISystem;

	/// <summary>
	/// Adds an <see cref="ISystem"/> with the given <see cref="Type"/> to the <see cref="ISystemManager"/>.
	/// </summary>
	/// <param name="type">The <see cref="ISystem"/> <see cref="Type"/>.</param>
	/// <returns></returns>
	ISystem Add(Type type);

	/// <summary>
	/// Removes an <see cref="ISystem"/> with the given <see cref="Type"/> from the <see cref="ISystemManager"/>.
	/// </summary>
	/// <typeparam name="TSystem">The <see cref="ISystem"/> generic <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	bool Remove<TSystem>()
		where TSystem : class, ISystem;

	/// <summary>
	/// Removes an <see cref="ISystem"/> with the given <see cref="Type"/> from the <see cref="ISystemManager"/>.
	/// </summary>
	/// <param name="type">The <see cref="ISystem"/> <see cref="Type"/>.</param>
	/// <returns></returns>
	bool Remove(Type type);

	/// <summary>
	/// Returns the <see cref="ISystem"/> with the given <see cref="Type"/>.
	/// </summary>
	/// <typeparam name="TSystem">The <see cref="ISystem"/> <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	TSystem Get<TSystem>() where TSystem : class, ISystem;

	/// <summary>
	/// Returns the <see cref="ISystem"/> with the given <see cref="Type"/>.
	/// </summary>
	/// <param name="type">The <see cref="ISystem"/> <see cref="Type"/>.</param>
	/// <returns></returns>
	ISystem Get(Type type);

	/// <summary>
	/// Returns the <see cref="ISystem"/> at the given index.
	/// <para>Systems are ordered and updated by their <see cref="ISystem.Priority"/>.</para>
	/// </summary>
	/// <param name="index">The <see cref="ISystem"/> index.</param>
	/// <returns></returns>
	ISystem Get(int index);

	/// <summary>
	/// Returns if the <see cref="ISystemManager"/> is managing a <see cref="ISystem"/> with the given <see cref="Type"/>.
	/// </summary>
	/// <typeparam name="TSystem">The <see cref="ISystem"/> <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	bool Has<TSystem>() where TSystem : class, ISystem;

	/// <summary>
	/// Returns if the <see cref="ISystemManager"/> is managing a <see cref="ISystem"/> with the given <see cref="Type"/>.
	/// </summary>
	/// <param name="type">The <see cref="ISystem"/> <see cref="Type"/>.</param>
	/// <returns></returns>
	bool Has(Type type);

	/// <summary>
	/// Returns if the <see cref="ISystemManager"/> is managing a <see cref="ISystem"/> with the given instance.
	/// </summary>
	/// <param name="system">The <see cref="ISystem"/> instance.</param>
	/// <returns></returns>
	bool Has(ISystem system);
}