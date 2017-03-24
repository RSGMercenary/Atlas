using Atlas.Engine.Collections.Fixed;
using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Components
{
	interface IEngine : IComponent
	{
		FixedStack<IEntity> EntityPool { get; }
		FixedStack<IFamily> FamilyPool { get; }

		/// <summary>
		/// A collection of all Entities managed by this Engine.
		/// <para>Entities are added to and removed from the Engine by being
		/// added as a child of an Entity already in the Entity hierarchy.
		/// This is done by creating a root Entity, adding an Engine Component
		/// to it, and then adding children to that hierarchy.</para>
		/// </summary>
		IReadOnlyLinkList<IEntity> Entities { get; }

		/// <summary>
		/// A collection of all Systems managed by this Engine.
		/// <para>Systems are added to and removed from the Engine by being managed
		/// as a Type to an Entity already in the Entity hierarchy.</para>
		/// </summary>
		IReadOnlyLinkList<ISystem> Systems { get; }

		/// <summary>
		/// A collection of all Families managed by this Engine.
		/// 
		/// <para>Families of Entities are added to and removed from the Engine by
		/// being managed by a System intent on updating that Family.</para>
		/// </summary>
		IReadOnlyLinkList<IFamily> Families { get; }

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
		/// Returns a new or unpooled Entity to be used in the Engine.
		/// </summary>
		/// <param name="managed">Whether the Entity should be managed.
		/// Managed Entities are returned to the pool once they've been destroyed.</param>
		/// <param name="globalName">The global name that will be given to the Entity.</param>
		/// <param name="localName">The local name the will be given to the Entity.</param>
		/// <returns></returns>
		IEntity GetEntity(bool managed = true, string globalName = "", string localName = "");

		/// <summary>
		/// Returns the Entity with the given global name.
		/// Every Entity has its own unique global name that can't be duplicated.
		/// </summary>
		/// <param name="globalName"></param>
		/// <returns></returns>
		IEntity GetEntity(string globalName);

		/// <summary>
		/// A Signal dispatching when an Entity has been added to the Engine.
		/// </summary>
		ISignal<IEngine, IEntity> EntityAdded { get; }

		/// <summary>
		/// A Signal dispatching when an Entity has been removed from the Engine.
		/// </summary>
		ISignal<IEngine, IEntity> EntityRemoved { get; }

		bool AddSystemType<TISystem, TSystem>()
			where TISystem : ISystem
			where TSystem : TISystem;

		bool AddSystemType(Type type, Type instance);

		bool RemoveSystemType<TISystem>()
			where TISystem : ISystem;

		bool RemoveSystemType(Type type);

		/// <summary>
		/// Returns if the Engine is managing a System with the given instance.
		/// </summary>
		/// <param name="system"></param>
		/// <returns></returns>
		bool HasSystem(ISystem system);

		/// <summary>
		/// Returns if the Engine is managing a System with the given Type.
		/// </summary>
		/// <typeparam name="TSystem"></typeparam>
		/// <returns></returns>
		bool HasSystem<TSystem>() where TSystem : ISystem;

		/// <summary>
		/// Returns if the Engine is managing a System with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasSystem(Type type);

		/// <summary>
		/// Returns the System with the given Type.
		/// </summary>
		/// <typeparam name="TSystem"></typeparam>
		/// <returns></returns>
		TSystem GetSystem<TSystem>() where TSystem : ISystem;

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

		/// <summary>
		/// A Signal dispatching when a System has been added to the Engine.
		/// </summary>
		ISignal<IEngine, ISystem, Type> SystemAdded { get; }

		/// <summary>
		/// A Signal dispatching when a System has been removed from the Engine.
		/// </summary>
		ISignal<IEngine, ISystem, Type> SystemRemoved { get; }

		ISystem CurrentUpdateSystem { get; }
		ISystem CurrentFixedUpdateSystem { get; }

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
		bool HasFamily<TFamilyType>();

		/// <summary>
		/// Returns if the Engine is managing a Family with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool HasFamily(Type type);

		/// <summary>
		/// Returns the Family with the given Type.
		/// </summary>
		/// <typeparam name="TFamilyType"></typeparam>
		/// <returns></returns>
		IFamily GetFamily<TFamilyType>();

		/// <summary>
		/// Returns the Family with the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IFamily GetFamily(Type type);

		IFamily AddFamily<TFamilyType>();
		IFamily AddFamily(Type type);

		IFamily RemoveFamily<TFamilyType>();
		IFamily RemoveFamily(Type type);

		ISignal<IEngine, Type> FamilyAdded { get; }
		ISignal<IEngine, Type> FamilyRemoved { get; }

		/// <summary>
		/// The delta time between <see cref="ISystem.Update"/> loops.
		/// </summary>
		float DeltaUpdateTime { get; }

		/// <summary>
		/// The total time spent running <see cref="ISystem.Update"/> loops.
		/// </summary>
		float TotalUpdateTime { get; }

		/// <summary>
		/// The fixed delta time between <see cref="ISystem.FixedUpdate"/> loops.
		/// </summary>
		float DeltaFixedUpdateTime { get; set; }

		/// <summary>
		/// The total time spent running <see cref="ISystem.FixedUpdate"/> loops.
		/// </summary>
		float TotalFixedUpdateTime { get; }

		/// <summary>
		/// The delta time between <see cref="ISystem.FixedUpdate"/> and
		/// <see cref="ISystem.Update"/> loops.
		/// </summary>
		float DeltaEngineTime { get; }

		/// <summary>
		/// The total time spent running <see cref="ISystem.FixedUpdate"/> and
		/// <see cref="ISystem.Update"/> loops.
		/// </summary>
		float TotalEngineTime { get; }

		/// <summary>
		/// Returns if the Engine is currently running. This is true
		/// if the Engine is running, regardless of whether the Engine is
		/// currently engaged in an update cycle.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Begins running the Engine once all manual Engine/Entity initialization
		/// has been completed. Once this is called, the Engine and its
		/// Entities, Systems, and Families should run itself.
		/// </summary>
		void Run();

		/// <summary>
		/// Returns if the Engine is currently engaged in an update cycle.
		/// </summary>
		bool IsUpdating { get; }

		ISignal<IEngine, bool> IsUpdatingChanged { get; }
	}
}