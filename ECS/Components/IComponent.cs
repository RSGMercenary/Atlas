using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using Atlas.Framework.Collections.EngineList;
using System;

namespace Atlas.ECS.Components
{
	public interface IComponent : IAutoDestroyObject
	{
		/// <summary>
		/// Determines whether <see cref="IObject.Dispose"/> is automatically called when <see cref="Managers"/>.Count == 0.
		/// </summary>
		new bool AutoDestroy { get; set; }

		/// <summary>
		/// Gets the index of the Entity. Returns -1 if the Entity
		/// isn't found in this Component.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		int GetManagerIndex(IEntity entity);

		/// <summary>
		/// Sets the index of the Entity. Returns true if successful. Returns false if
		/// the Entity isn't found in this Component.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		bool SetManagerIndex(IEntity entity, int index);

		/// <summary>
		/// Swaps the indices of the Entities. Returns true if successful. Returns
		/// false if the Entities are null or aren't in this Component.
		/// </summary>
		/// <param name="entity1">The first Entity to swap.</param>
		/// <param name="entity2">The second Entity to swap.</param>
		/// <returns></returns>
		bool SwapManagers(IEntity entity1, IEntity entity2);

		/// <summary>
		/// Swaps the indices of the Entities. Returns true if successful. Returns
		/// false if the Entities are null or aren't in this Component.
		/// </summary>
		/// <param name="index1">The index of the first Entity.</param>
		/// <param name="index2">The index of the second Entity.</param>
		/// <returns></returns>
		bool SwapManagers(int index1, int index2);

		/// <summary>
		/// Adds an Entity to this Component.
		/// </summary>
		/// <param name="entity">The Entity to add to this Component.</param>
		/// <returns></returns>
		IEntity AddManager(IEntity entity);

		/// <summary>
		/// Adds an Entity to this Component with the given Type. The Type
		/// is used for Component lookup in the Entity and must be a subclass
		/// or interface if this Component.
		/// </summary>
		/// <param name="entity">The Entity to add to this Component.</param>
		/// <param name="type">A Type used for Component lookup in the Entity 
		/// that is a subclass or interface of this Component.</param>
		/// <returns></returns>
		IEntity AddManager(IEntity entity, Type type);

		/// <summary>
		/// Adds an Entity to this Component at the given index.
		/// </summary>
		/// <param name="entity">The Entity to add to this Component.</param>
		/// <param name="index">The index of the Entity in this Component.</param>
		/// <returns></returns>
		IEntity AddManager(IEntity entity, int index);

		/// <summary>
		/// Adds an Entity to this Component with the given Type and index. The Type
		/// is used for Component lookup in the Entity and must be a subclass
		/// or interface if this Component.
		/// </summary>
		/// <param name="entity">The Entity to add to this Component.</param>
		/// <param name="type">A Type used for Component lookup in the Entity 
		/// that is a subclass or interface of this Component.</param>
		/// <param name="index">The index of the Entity in this Component.</param>
		/// <returns></returns>
		IEntity AddManager(IEntity entity, Type type = null, int index = int.MaxValue);

		/// <summary>
		/// Adds an Entity to this Component with the given TIComponent Type. The Type
		/// is used for Component lookup in the Entity and must be a subclass
		/// or interface if this Component.
		/// </summary>
		/// <typeparam name="TIComponent">A Type used for Component lookup in the Entity 
		/// that is a subclass or interface of this Component.</typeparam>
		/// <param name="entity">The Entity to add to this Component.</param>
		/// <returns></returns>
		IEntity AddManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent;

		/// <summary>
		/// Adds an Entity to this Component with the given Type and index. The Type
		/// is used for Component lookup in the Entity and must be a subclass
		/// or interface if this Component.
		/// </summary>
		/// <typeparam name="TIComponent">A Type used for Component lookup in the Entity 
		/// that is a subclass or interface of this Component.</typeparam>
		/// <param name="entity">The Entity to add to this Component.</param>
		/// <param name="index">The index of the Entity in this Component.</param>
		/// <returns></returns>
		IEntity AddManager<TIComponent>(IEntity entity, int index = int.MaxValue)
			where TIComponent : IComponent;

		/// <summary>
		/// Removes an Entity from this Component.
		/// </summary>
		/// <param name="entity">The Entity to remove from this Component.</param>
		/// <returns></returns>
		IEntity RemoveManager(IEntity entity);

		/// <summary>
		/// Removes an Entity from this Component with the given Type. The Type
		/// is used for Component lookup in the Entity and must be a subclass
		/// or interface if this Component.
		/// </summary>
		/// <param name="entity">The Entity to remove from this Component.</param>
		/// <param name="type">A Type used for Component lookup in the Entity 
		/// that is a subclass or interface of this Component.</param>
		/// <returns></returns>
		IEntity RemoveManager(IEntity entity, Type type);

		/// <summary>
		/// Removes an Entity from this Component at the given index.
		/// </summary>
		/// <param name="index">The index of the Entity in this Component.</param>
		/// <returns></returns>
		IEntity RemoveManager(int index);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TIComponent"></typeparam>
		/// <param name="entity">The Entity to remove from this Component.</param>
		/// <returns></returns>
		IEntity RemoveManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent;

		/// <summary>
		/// Removes all Entities managing this Component. Returns true if successful.
		/// Returns false if no Entities were managing this Component.
		/// </summary>
		/// <returns></returns>
		bool RemoveManagers();

		/// <summary>
		/// The Entity managing this Component. This is always null if
		/// the Component is shareable, or only null if no Entity has been
		/// added yet.
		/// </summary>
		IEntity Manager { get; }

		/// <summary>
		/// The Entities managing this Component. This will return all Entities
		/// regardless of the Component's shareable status.
		/// </summary>
		IReadOnlyEngineList<IEntity> Managers { get; }

		/// <summary>
		/// A Boolean of whether this Component is shareable. Shareable Components
		/// are able to be added to multiple Entities at once and can contain
		/// information that is shared between similar Entities.
		/// </summary>
		bool IsShareable { get; }

		/// <summary>
		/// A custom ToString() implementation used in conjunction with an Entity's custom ToString()
		/// implementation to print out detailed statistics during runtime.
		/// </summary>
		/// <param name="addManagers">Adds details about the Entities managing this Component.</param>
		/// <param name="index">The index of this Component when being printed with an Entity's ToString().</param>
		/// <param name="indent">Indentation used for formatting the ToString() correctly.</param>
		/// <returns></returns>
		string ToInfoString(bool addManagers = true, int index = 0, string indent = "");
	}
}
