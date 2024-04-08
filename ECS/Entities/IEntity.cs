using Atlas.Core.Messages;
using Atlas.Core.Objects.AutoDispose;
using Atlas.Core.Objects.Sleep;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Entities;

public interface IEntity : IHierarchyMessenger<IEntity>, IEngineItem, IAutoDispose, ISleep
{
	#region Entities
	/// <summary>
	/// Determines whether <see cref="IDisposable.Dispose"/> is called when <see cref="Parent"/> == <see langword="null"/>.
	/// </summary>
	new bool IsAutoDisposable { get; set; }

	/// <summary>
	/// This <see cref="IEntity"/>'s global name. This name is unique to its Engine.
	/// If this Entity is added to an Engine, and this global name already
	/// exists, then this Entity's global name will be changed.
	/// </summary>
	string GlobalName { get; set; }

	/// <summary>
	/// This <see cref="IEntity"/>'s local name. This name is unique to its Parent.
	/// If this Entity is added to a parent, and this local name already
	/// exists, then this Entity's local name will be changed.
	/// </summary>
	string LocalName { get; set; }

	IEntity AddChild(string globalName, string localName);
	IEntity AddChild(string globalName, string localName, int index);
	IEntity AddChild(string globalName);
	IEntity AddChild(string globalName, int index);

	bool HasChild(string localName);

	/// <summary>
	/// Returns the <see cref="IEntity"/> in the given <paramref name="hierarchy"/>, starting
	/// with this <see cref="IEntity"/> and traversing parents and/or children until the right
	/// <see cref="IEntity"/> is found.
	/// <para>".." is the parent, and "/" is the name separator.</para>
	/// </summary>
	/// <param name="hierarchy">A hierarchy string in format of "../../name1/name2/name3".</param>
	/// <returns></returns>
	IEntity GetRelative(string hierarchy);

	IEntity SetRelative(string hierarchy, int index);

	IEntity GetChild(string localName);

	IEntity RemoveChild(string localName);
	#endregion

	#region Components
	#region Has
	bool HasComponent<TKey>()
		where TKey : IComponent;

	bool HasComponent(Type type);
	#endregion

	#region Get
	TValue GetComponent<TKey, TValue>()
		where TKey : IComponent
		where TValue : class, TKey;

	TKeyValue GetComponent<TKeyValue>()
		where TKeyValue : class, IComponent;

	IComponent GetComponent(Type type);

	TValue GetComponent<TValue>(Type type)
		where TValue : class, IComponent;

	Type GetComponentType(IComponent component);

	IReadOnlyDictionary<Type, IComponent> Components { get; }

	TKeyValue GetAncestorComponent<TKeyValue>(int depth, bool self)
		where TKeyValue : class, IComponent;

	IEnumerable<TKeyValue> GetDescendantComponents<TKeyValue>(int depth)
		where TKeyValue : class, IComponent;
	#endregion

	#region Add
	/// <summary>
	/// Adds a Component to the Entity with a new instance.
	/// </summary>
	/// <typeparam name="TKey">The interface Type of the Component.</typeparam>
	/// <typeparam name="TValue">The instance Type of the Component.</typeparam>
	/// <returns></returns>
	TValue AddComponent<TKey, TValue>()
		where TKey : IComponent
		where TValue : class, TKey, new();

	/// <summary>
	/// Adds a Component to the Entity with the given instance.
	/// </summary>
	/// <typeparam name="TKey">The interface Type of the Component.</typeparam>
	/// <typeparam name="TValue">The instance Type of the Component.</typeparam>
	/// <param name="Component">The instance of the Component.</param>
	/// <returns></returns>
	TValue AddComponent<TKey, TValue>(TValue component)
		where TKey : IComponent
		where TValue : class, TKey;

	/// <summary>
	/// Adds a Component to the Entity with the given instance and index.
	/// </summary>
	/// <typeparam name="TKey">The interface Type of the Component.</typeparam>
	/// <typeparam name="TValue">The instance Type of the Component.</typeparam>
	/// <param name="Component">The instance of the Component.</param>
	/// <param name="index">The index of the Entity within the Component.</param>
	/// <returns></returns>
	TValue AddComponent<TKey, TValue>(TValue component, int index)
		where TKey : IComponent
		where TValue : class, TKey;

	/// <summary>
	/// Adds a Component to the Entity with a new instance.
	/// </summary>
	/// <typeparam name="TKeyValue">The instance Type of the Component.</typeparam>
	/// <returns></returns>
	TKeyValue AddComponent<TKeyValue>()
		where TKeyValue : class, IComponent, new();

	/// <summary>
	/// Adds a Component to the Entity with the given instance.
	/// </summary>
	/// <typeparam name="TKeyValue">The interface Type of the Component.</typeparam>
	/// <param name="Component">The instance of the Component.</param>
	/// <returns></returns>
	TKeyValue AddComponent<TKeyValue>(TKeyValue component)
		where TKeyValue : class, IComponent;

	/// <summary>
	/// Adds a Component to the Entity with the given instance and index.
	/// </summary>
	/// <typeparam name="TKeyValue">The interface Type of the Component.</typeparam>
	/// <param name="component">The instance of the Component.</param>
	/// <param name="index">The index of the Entity within the Component.</param>
	/// <returns></returns>
	TKeyValue AddComponent<TKeyValue>(TKeyValue component, int index)
		where TKeyValue : class, IComponent;

	TValue AddComponent<TValue>(TValue component, Type type)
		where TValue : class, IComponent;

	TValue AddComponent<TValue>(Type type)
		where TValue : class, IComponent, new();

	IComponent AddComponent(IComponent component);
	IComponent AddComponent(IComponent component, Type type);
	IComponent AddComponent(IComponent component, int index);
	IComponent AddComponent(IComponent component, Type type, int index);
	#endregion

	#region Remove
	TValue RemoveComponent<TKey, TValue>()
		where TKey : IComponent
		where TValue : class, TKey;

	TKeyValue RemoveComponent<TKeyValue>()
		where TKeyValue : class, IComponent;

	IComponent RemoveComponent(Type type);

	IComponent RemoveComponent(IComponent component);

	bool RemoveComponents();
	#endregion
	#endregion

	#region Sleeping
	/// <summary>
	/// Return whether this Entity.SelfSleeping > 0;
	/// </summary>
	bool IsSelfSleeping { get; set; }

	/// <summary>
	/// This Entity's self-sleeping count. If this Entity is self-sleeping,
	/// its sleeping is not changed by its parent sleeping.
	/// </summary>
	int SelfSleeping { get; }
	#endregion

	IComponent AddComponentAsInterface(IComponent component);

	IComponent AddComponentAsInterface(IComponent component, int index);
}