using Atlas.Core.Collections.EngineList;
using Atlas.Core.Objects;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components
{
	public interface IEngine : IComponent, IUpdateObject
	{
		#region Entities

		/// <summary>
		/// A collection of all Entities managed by this Engine.
		/// <para>Entities are added to and removed from the Engine by being
		/// added as a child of an Entity already in the Entity hierarchy.
		/// This is done by creating a root Entity, adding an Engine Component
		/// to it, and then adding children to that hierarchy.</para>
		/// </summary>
		IReadOnlyEngineList<IEntity> Entities { get; }

		/// <summary>
		/// Returns if the Engine is managing an Entity with the given global name.
		/// Every Entity has its own unique global name that can't be duplicated.
		/// </summary>
		/// <param name="globalName">The global name of the Entity.</param>
		/// <returns></returns>
		bool HasEntity(string globalName);

		/// <summary>
		/// Returns if the Engine is managing an Entity with the given instance.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool HasEntity(IEntity entity);

		/// <summary>
		/// Returns the Entity with the given global name.
		/// Every Entity has its own unique global name that can't be duplicated.
		/// </summary>
		/// <param name="globalName"></param>
		/// <returns></returns>
		IEntity GetEntity(string globalName);

		#endregion

		#region Systems

		/// <summary>
		/// A collection of all Systems managed by this Engine.
		/// <para>Systems are added to and removed from the Engine by being managed
		/// as a Type to an Entity already in the Entity hierarchy.</para>
		/// </summary>
		IReadOnlyEngineList<IReadOnlySystem> Systems { get; }

		bool AddSystemType<TISystem, TSystem>()
			where TISystem : ISystem
			where TSystem : TISystem, new();

		bool AddSystemType(Type type, Type instance);

		bool RemoveSystemType<TISystem>()
			where TISystem : IReadOnlySystem;

		bool RemoveSystemType(Type type);

		/// <summary>
		/// Returns if the Engine is managing a System with the given instance.
		/// </summary>
		/// <param name="system"></param>
		/// <returns></returns>
		bool HasSystem(IReadOnlySystem system);

		/// <summary>
		/// Returns if the Engine is managing a System with the given Type.
		/// </summary>
		/// <typeparam name="TISystem"></typeparam>
		/// <returns></returns>
		bool HasSystem<TISystem>() where TISystem : IReadOnlySystem;

		/// <summary>
		/// Returns if the Engine is managing a System with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasSystem(Type type);

		/// <summary>
		/// Returns the System with the given Type.
		/// </summary>
		/// <typeparam name="TISystem"></typeparam>
		/// <returns></returns>
		TISystem GetSystem<TISystem>() where TISystem : IReadOnlySystem;

		/// <summary>
		/// Returns the System with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IReadOnlySystem GetSystem(Type type);

		/// <summary>
		/// Returns the System at the given index. Systems are order
		/// and updated by their priority.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IReadOnlySystem GetSystem(int index);

		#endregion

		#region Families

		/// <summary>
		/// A collection of all Families managed by this Engine.
		/// 
		/// <para>Families of Entities are added to and removed from the Engine by
		/// being managed by a System intent on updating that Family.</para>
		/// </summary>
		IReadOnlyEngineList<IReadOnlyFamily> Families { get; }

		/// <summary>
		/// Returns if the Engine is managing a Family with the given instance.
		/// </summary>
		/// <param name="family"></param>
		/// <returns></returns>
		bool HasFamily(IReadOnlyFamily family);

		/// <summary>
		/// Returns if the Engine is managing a Family with the given Type.
		/// </summary>
		/// <typeparam name="TFamilyType"></typeparam>
		/// <returns></returns>
		bool HasFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new();

		/// <summary>
		/// Returns if the Engine is managing a Family with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasFamily(Type type);

		/// <summary>
		/// Returns the Family with the given Type.
		/// </summary>
		/// <typeparam name="TFamilyMember"></typeparam>
		/// <returns></returns>
		IReadOnlyFamily<TFamilyMember> GetFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new();

		/// <summary>
		/// Returns the Family with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IReadOnlyFamily GetFamily(Type type);

		IReadOnlyFamily<TFamilyMember> AddFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new();

		IReadOnlyFamily<TFamilyMember> RemoveFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new();

		#endregion

		/// <summary>
		/// The delta time between update loops. This is updated every loop.
		/// </summary>
		double DeltaTime { get; }

		/// <summary>
		/// The total time spent running update loops. This is updated every loop.
		/// </summary>
		double TotalTime { get; }

		/// <summary>
		/// A Dictionary of fixed times and fixed time update totals.
		/// </summary>
		IReadOnlyDictionary<double, double> FixedTimes { get; }

		/// <summary>
		/// Returns if the Engine is currently running. This is true
		/// if the Engine is running, regardless of whether the Engine is
		/// currently engaged in an update cycle.
		/// </summary>
		bool IsRunning { get; set; }

		/// <summary>
		/// The current System that's undergoing an Update().
		/// </summary>
		IReadOnlySystem CurrentSystem { get; }
	}
}