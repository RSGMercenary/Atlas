using Atlas.Core.Collections.Group;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine.Systems;

public interface ISystemManager : IReadOnlyEngineObject
{
	event Action<ISystemManager, ISystem, Type> Added;

	event Action<ISystemManager, ISystem, Type> Removed;

	TSystem Add<TSystem>()
		where TSystem : class, ISystem;

	ISystem Add(Type type);

	bool Remove<TSystem>()
		where TSystem : class, ISystem;

	bool Remove(Type type);

	/// <summary>
	/// A collection of all Systems managed by this Engine.
	/// <para>Systems are added to and removed from the Engine by being managed
	/// as a Type to an Entity already in the Entity hierarchy.</para>
	/// </summary>
	IReadOnlyGroup<ISystem> Systems { get; }

	/// <summary>
	/// Returns if the Engine is managing a System with the given instance.
	/// </summary>
	/// <param name="system"></param>
	/// <returns></returns>
	bool Has(ISystem system);

	/// <summary>
	/// Returns if the Engine is managing a System with the given Type.
	/// </summary>
	/// <typeparam name="TSystem"></typeparam>
	/// <returns></returns>
	bool Has<TSystem>() where TSystem : ISystem;

	/// <summary>
	/// Returns if the Engine is managing a System with the given Type.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	bool Has(Type type);

	/// <summary>
	/// Returns the System with the given Type.
	/// </summary>
	/// <typeparam name="TSystem"></typeparam>
	/// <returns></returns>
	TSystem Get<TSystem>() where TSystem : ISystem;

	/// <summary>
	/// Returns the System with the given Type.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	ISystem Get(Type type);

	/// <summary>
	/// Returns the System at the given index. Systems are order
	/// and updated by their priority.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	ISystem Get(int index);
}