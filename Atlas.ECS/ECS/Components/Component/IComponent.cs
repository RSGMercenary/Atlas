using Atlas.Core.Collections.Group;
using Atlas.Core.Messages;
using Atlas.Core.Objects.AutoDispose;
using Atlas.ECS.Entities;
using Atlas.ECS.Serialization;
using System;

namespace Atlas.ECS.Components.Component;

public interface IComponent : IMessenger, IAutoDispose, ISerialize
{
	/// <summary>
	/// Determines whether <see cref="IDisposable.Dispose"/> is automatically called when <see cref="Managers"/>.Count == 0.
	/// </summary>
	new bool IsAutoDisposable { get; set; }

	bool HasManager(IEntity entity);

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
	IEntity AddManager(IEntity entity, Type type, int index);

	/// <summary>
	/// Adds an Entity to this Component with the given TIComponent Type. The Type
	/// is used for Component lookup in the Entity and must be a subclass
	/// or interface if this Component.
	/// </summary>
	/// <typeparam name="TKey">A Type used for Component lookup in the Entity 
	/// that is a subclass or interface of this Component.</typeparam>
	/// <param name="entity">The Entity to add to this Component.</param>
	/// <returns></returns>
	IEntity AddManager<TKey>(IEntity entity)
		where TKey : IComponent;

	/// <summary>
	/// Adds an Entity to this Component with the given Type and index. The Type
	/// is used for Component lookup in the Entity and must be a subclass
	/// or interface if this Component.
	/// </summary>
	/// <typeparam name="TKey">A Type used for Component lookup in the Entity 
	/// that is a subclass or interface of this Component.</typeparam>
	/// <param name="entity">The Entity to add to this Component.</param>
	/// <param name="index">The index of the Entity in this Component.</param>
	/// <returns></returns>
	IEntity AddManager<TKey>(IEntity entity, int index)
		where TKey : IComponent;

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
	/// <typeparam name="TKey"></typeparam>
	/// <param name="entity">The Entity to remove from this Component.</param>
	/// <returns></returns>
	IEntity RemoveManager<TKey>(IEntity entity)
		where TKey : IComponent;

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
	IReadOnlyGroup<IEntity> Managers { get; }

	/// <summary>
	/// A Boolean of whether this Component is shareable. Shareable Components
	/// are able to be added to multiple Entities at once and can contain
	/// information that is shared between similar Entities.
	/// </summary>
	bool IsShareable { get; }
}

public interface IComponent<T> : IMessenger<T>, IComponent where T : IComponent { }