using Atlas.Core.Collections.Group;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine
{
	public interface IEngine : IComponent<IEngine>, IUpdateState
	{
		#region Entities

		/// <summary>
		/// A collection of all Entities managed by this Engine.
		/// <para>Entities are added to and removed from the Engine by being
		/// added as a child of an Entity already in the Entity hierarchy.
		/// This is done by creating a root Entity, adding an Engine Component
		/// to it, and then adding children to that hierarchy.</para>
		/// </summary>
		IReadOnlyGroup<IEntity> Entities { get; }

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

		TSystem AddSystem<TSystem>()
			where TSystem : class, ISystem, new();

		ISystem AddSystem(Type type);

		void RemoveSystem<TSystem>()
			where TSystem : class, ISystem, new();

		void RemoveSystem(Type type);

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
		bool HasSystem(ISystem system);

		/// <summary>
		/// Returns if the Engine is managing a System with the given Type.
		/// </summary>
		/// <typeparam name="TISystem"></typeparam>
		/// <returns></returns>
		bool HasSystem<TISystem>() where TISystem : ISystem;

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
		TISystem GetSystem<TISystem>() where TISystem : ISystem;

		/// <summary>
		/// Returns the System with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ISystem GetSystem(Type type);

		/// <summary>
		/// Returns the System at the given index. Systems are order
		/// and updated by their priority.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		ISystem GetSystem(int index);

		#endregion

		#region Families

		/// <summary>
		/// A collection of all Families managed by this Engine.
		/// 
		/// <para>Families of Entities are added to and removed from the Engine by
		/// being managed by a System intent on updating that Family.</para>
		/// </summary>
		IReadOnlyGroup<IFamily> Families { get; }

		/// <summary>
		/// Returns if the Engine is managing a Family with the given instance.
		/// </summary>
		/// <param name="family"></param>
		/// <returns></returns>
		bool HasFamily(IFamily family);

		/// <summary>
		/// Returns if the Engine is managing a Family with the given Type.
		/// </summary>
		/// <typeparam name="TFamilyType"></typeparam>
		/// <returns></returns>
		bool HasFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new();

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
		IFamily<TFamilyMember> GetFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new();

		/// <summary>
		/// Returns the Family with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IFamily GetFamily(Type type);

		IFamily<TFamilyMember> AddFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new();

		void RemoveFamily<TFamilyMember>()
			where TFamilyMember : class, IFamilyMember, new();

		#endregion

		#region Updates

		/// <summary>
		/// The max delta time between updates. Prevents the update loop "spiral of death".
		/// </summary>
		double MaxVariableTime { get; set; }

		/// <summary>
		/// The delta time between variable-time updates. This is set every loop.
		/// </summary>
		double DeltaVariableTime { get; }

		/// <summary>
		/// The total time running variable-time updates. This is set every loop.
		/// </summary>
		double TotalVariableTime { get; }

		/// <summary>
		/// The delta time between fixed-time updates. This is set manually.
		/// </summary>
		double DeltaFixedTime { get; set; }

		/// <summary>
		/// The total time running fixed-time updates. This is set every loop.
		/// </summary>
		double TotalFixedTime { get; }

		/// <summary>
		/// The current System that's undergoing an Update().
		/// </summary>
		ISystem CurrentSystem { get; }

		#endregion
	}
}