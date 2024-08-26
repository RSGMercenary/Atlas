using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Objects.AutoDispose;
using Atlas.Core.Objects.Sleep;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Serialization;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Entities;

public interface IEntity : IEngineObject<IEntity>, IHierarchy<IEntity>, ISleep<IEntity>, IAutoDispose<IEntity>, IDisposable, ISerialize
{
	event Action<IEntity, IComponent, Type> ComponentAdded;
	event Action<IEntity, IComponent, Type> ComponentRemoved;
	event Action<IEntity, string, string> GlobalNameChanged;
	event Action<IEntity, string, string> LocalNameChanged;
	event Action<IEntity, int, int> FreeSleepingChanged;

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
	bool HasComponent<TType>()
		where TType : class, IComponent;

	bool HasComponent(Type type);
	#endregion

	#region Get
	TComponent GetComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType;

	TType GetComponent<TType>()
		where TType : class, IComponent;

	IComponent GetComponent(Type type);

	TComponent GetComponent<TComponent>(Type type)
		where TComponent : class, IComponent;

	Type GetComponentType(IComponent component);

	IReadOnlyDictionary<Type, IComponent> Components { get; }

	TType GetAncestorComponent<TType>(int depth, bool self)
		where TType : class, IComponent;

	IEnumerable<TType> GetDescendantComponents<TType>(int depth)
		where TType : class, IComponent;
	#endregion

	#region Add
	#region Component & Type
	/// <summary>
	/// Adds a Component to the Entity with a new instance.
	/// </summary>
	/// <typeparam name="TType">The interface Type of the Component.</typeparam>
	/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
	/// <returns></returns>
	TComponent AddComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType, new();

	/// <summary>
	/// Adds a Component to the Entity with the given instance and index.
	/// </summary>
	/// <typeparam name="TType">The interface Type of the Component.</typeparam>
	/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
	/// <param name="Component">The instance of the Component.</param>
	/// <param name="index">The index of the Entity within the Component.</param>
	/// <returns></returns>
	TComponent AddComponent<TComponent, TType>(TComponent component, int index)
		where TType : class, IComponent
		where TComponent : class, TType;

	TComponent AddComponent<TComponent, TType>(TComponent component)
		where TType : class, IComponent
		where TComponent : class, TType;
	#endregion

	#region Component
	/// <summary>
	/// Adds a Component to the Entity with a new instance.
	/// </summary>
	/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
	/// <returns></returns>
	TComponent AddComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent, new();

	TComponent AddComponent<TComponent>(TComponent component, int index)
		where TComponent : class, IComponent;

	/// <summary>
	/// Adds a Component to the Entity with the given instance and index.
	/// </summary>
	/// <typeparam name="TType">The interface Type of the Component.</typeparam>
	/// <param name="component">The instance of the Component.</param>
	/// <param name="index">The index of the Entity within the Component.</param>
	/// <returns></returns>
	//TType AddComponent<TType>(TType component, int index)
	//where TType : class, IComponent;
	TComponent AddComponent<TComponent>(TComponent component, Type type = null, int? index = null)
		where TComponent : class, IComponent;

	IComponent AddComponentType(IComponent component, int? index = null);
	#endregion
	#endregion

	#region Remove
	TComponent RemoveComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType;

	TComponent RemoveComponent<TComponent>(TComponent component, Type type = null)
		where TComponent : class, IComponent;

	TComponent RemoveComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent;

	TComponent RemoveComponentType<TComponent>(TComponent component)
		where TComponent : class, IComponent;

	IComponent RemoveComponent(Type type);

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
}