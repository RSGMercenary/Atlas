using Atlas.Core.Collections.LinkList;
using Atlas.Core.Objects.AutoDispose;
using Atlas.ECS.Entities;
using Atlas.ECS.Serialization;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Component;

public interface IComponent : IEnumerable<IEntity>, IDisposable, ISerialize
{
	event Action<IComponent, IEntity> ManagerAdded;

	event Action<IComponent, IEntity> ManagerRemoved;

	event Action<IComponent> ManagersChanged;

	/// <summary>
	/// A Boolean of whether this Component is shareable. Shareable Components
	/// are able to be added to multiple Entities at once and can contain
	/// information that is shared between similar Entities.
	/// </summary>
	bool IsShareable { get; }

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

	#region Add
	/// <summary>
	/// Adds an <see cref="IEntity"/> to this <see cref="IComponent"/> at the given index.
	/// </summary>
	/// <param name="entity">The Entity to add to this Component.</param>
	/// <param name="index">The index of the Entity in this Component.</param>
	/// <returns></returns>
	IEntity AddManager(IEntity entity, int index);

	/// <summary>
	/// Adds an <see cref="IEntity"/> to this <see cref="IComponent"/> with the given <paramref name="type"/> and <paramref name="index"/>. The Type
	/// is used for Component lookup in the Entity and must be a subclass
	/// or interface if this Component.
	/// </summary>
	/// <param name="entity">The Entity to add to this Component.</param>
	/// <param name="type">A Type used for Component lookup in the Entity 
	/// that is a subclass or interface of this Component.</param>
	/// <param name="index">The index of the Entity in this Component.</param>
	/// <returns></returns>
	IEntity AddManager(IEntity entity, Type type = null, int? index = null);

	/// <summary>
	/// Adds an Entity to this Component with the given Type and index. The Type
	/// is used for Component lookup in the Entity and must be a subclass
	/// or interface if this Component.
	/// </summary>
	/// <typeparam name="TType">A Type used for Component lookup in the Entity 
	/// that is a subclass or interface of this Component.</typeparam>
	/// <param name="entity">The Entity to add to this Component.</param>
	/// <param name="index">The index of the Entity in this Component.</param>
	/// <returns></returns>
	IEntity AddManager<TType>(IEntity entity, int? index = null)
		where TType : class, IComponent;
	#endregion

	#region Remove
	/// <summary>
	/// Removes an Entity from this Component with the given Type. The Type
	/// is used for Component lookup in the Entity and must be a subclass
	/// or interface if this Component.
	/// </summary>
	/// <param name="entity">The Entity to remove from this Component.</param>
	/// <param name="type">A Type used for Component lookup in the Entity 
	/// that is a subclass or interface of this Component.</param>
	/// <returns></returns>
	IEntity RemoveManager(IEntity entity, Type type = null);

	/// <summary>
	/// Removes an Entity from this Component at the given index.
	/// </summary>
	/// <param name="index">The index of the Entity in this Component.</param>
	/// <returns></returns>
	IEntity RemoveManager(int index);

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	/// <param name="entity">The Entity to remove from this Component.</param>
	/// <returns></returns>
	IEntity RemoveManager<TType>(IEntity entity)
		where TType : class, IComponent;

	/// <summary>
	/// Removes all Entities managing this Component. Returns true if successful.
	/// Returns false if no Entities were managing this Component.
	/// </summary>
	/// <returns></returns>
	bool RemoveManagers();
	#endregion

	#region Managers
	/// <summary>
	/// The <see cref="IEntity"/> instance managing this <see cref="IComponent"/>.
	/// <para>Returns <see langword="null"/> if <see cref="IComponent.IsShareable"/> is <see langword="true"/>.</para>
	/// </summary>
	IEntity Manager { get; }

	/// <summary>
	/// The <see cref="IEntity"/> instances managing this <see cref="IComponent"/>.
	/// </summary>
	IReadOnlyLinkList<IEntity> Managers { get; }
	#endregion
}

public interface IComponent<out T> : IComponent, IAutoDispose<T> where T : IComponent<T>
{
	/// <summary>
	/// Determines whether <see cref="IDisposable.Dispose"/> is automatically called when <see cref="IComponent.Managers"/>.Count == 0.
	/// </summary>
	new bool IsAutoDisposable { get; set; }

	new event Action<T, IEntity> ManagerAdded;

	new event Action<T, IEntity> ManagerRemoved;

	new event Action<T> ManagersChanged;
}