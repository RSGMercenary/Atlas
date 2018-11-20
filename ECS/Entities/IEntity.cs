﻿using Atlas.Core.Collections.Group;
using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Entities
{
	public interface IEntity : IObject, IAutoDispose, ISleep
	{
		#region Entities

		/// <summary>
		/// Determines whether <see cref="IObject.Dispose"/> is called when <see cref="Parent"/> == <see langword="null"/>.
		/// </summary>
		new bool AutoDispose { get; set; }

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

		IEntity AddChild(string globalName = "", string localName = "");
		IEntity AddChild(int index, string globalName = "", string localName = "");

		bool HasChild(string localName);

		/// <summary>
		/// Returns the <see cref="IEntity"/> in the given <paramref name="hierarchy"/>, starting
		/// with this <see cref="IEntity"/> and traversing parents and/or children until the right
		/// <see cref="IEntity"/> is found.
		/// <para>".." is the parent, and "/" is the name separator.</para>
		/// </summary>
		/// <param name="hierarchy">A hierarchy string in format of "../../name1/name2/name3".</param>
		/// <returns></returns>
		IEntity GetHierarchy(string hierarchy);

		IEntity SetHierarchy(string hierarchy, int index);

		IEntity GetChild(string localName);

		IEntity RemoveChild(string localName);

		#endregion

		#region Hierarchy

		#region Root

		IEntity Root { get; }

		int RootIndex { get; }

		#endregion

		#region Parent

		IEntity Parent { get; set; }

		int ParentIndex { get; set; }

		IEntity SetParent(IEntity parent, int index);

		#endregion

		#region Get

		IReadOnlyGroup<IEntity> Children { get; }

		IEntity GetChild(int index);

		int GetChildIndex(IEntity child);

		#endregion

		#region Has

		bool HasDescendant(IEntity descendant);

		bool HasAncestor(IEntity ancestor);

		bool HasChild(IEntity child);

		bool HasSibling(IEntity sibling);

		#endregion

		#region Set

		bool SetChildIndex(IEntity child, int index);

		bool SwapChildren(IEntity child1, IEntity child2);

		bool SwapChildren(int index1, int index2);

		#endregion

		#region Add

		IEntity AddChild(IEntity child);

		IEntity AddChild(IEntity child, int index);

		#endregion

		#region Remove

		IEntity RemoveChild(IEntity child);

		IEntity RemoveChild(int index);

		bool RemoveChildren();

		#endregion

		#endregion

		#region Components

		#region Has

		bool HasComponent<TIComponent>()
			where TIComponent : IComponent;

		bool HasComponent(Type type);

		#endregion

		#region Get

		TComponent GetComponent<TIComponent, TComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent;

		TIComponent GetComponent<TIComponent>()
			where TIComponent : IComponent;

		IComponent GetComponent(Type type);

		TComponent GetComponent<TComponent>(Type type)
			where TComponent : IComponent;

		Type GetComponentType(IComponent component);

		IReadOnlyDictionary<Type, IComponent> Components { get; }

		#endregion

		#region Add

		/// <summary>
		/// Adds a Component to the Entity with a new instance.
		/// </summary>
		/// <typeparam name="TIComponent">The interface Type of the Component.</typeparam>
		/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
		/// <returns></returns>
		TComponent AddComponent<TComponent, TIComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent, new();

		/// <summary>
		/// Adds a Component to the Entity with the given instance.
		/// </summary>
		/// <typeparam name="TIComponent">The interface Type of the Component.</typeparam>
		/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <returns></returns>
		TComponent AddComponent<TComponent, TIComponent>(TComponent component)
			where TIComponent : IComponent
			where TComponent : TIComponent;

		/// <summary>
		/// Adds a Component to the Entity with the given instance and index.
		/// </summary>
		/// <typeparam name="TIComponent">The interface Type of the Component.</typeparam>
		/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <param name="index">The index of the Entity within the Component.</param>
		/// <returns></returns>
		TComponent AddComponent<TComponent, TIComponent>(TComponent component, int index)
			where TIComponent : IComponent
			where TComponent : TIComponent;

		/// <summary>
		/// Adds a Component to the Entity with a new instance.
		/// </summary>
		/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
		/// <returns></returns>
		TComponent AddComponent<TComponent>()
			where TComponent : IComponent, new();

		/// <summary>
		/// Adds a Component to the Entity with the given instance.
		/// </summary>
		/// <typeparam name="TIComponent">The interface Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <returns></returns>
		TIComponent AddComponent<TIComponent>(TIComponent component)
			where TIComponent : IComponent;

		/// <summary>
		/// Adds a Component to the Entity with the given instance and index.
		/// </summary>
		/// <typeparam name="TIComponent">The interface Type of the Component.</typeparam>
		/// <param name="component">The instance of the Component.</param>
		/// <param name="index">The index of the Entity within the Component.</param>
		/// <returns></returns>
		TIComponent AddComponent<TIComponent>(TIComponent component, int index)
			where TIComponent : IComponent;

		IComponent AddComponent(IComponent component);
		IComponent AddComponent(IComponent component, Type type);
		IComponent AddComponent(IComponent component, int index);
		IComponent AddComponent(IComponent component, Type type = null, int index = int.MaxValue);

		#endregion

		#region Remove

		TComponent RemoveComponent<TIComponent, TComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent;

		TIComponent RemoveComponent<TIComponent>()
			where TIComponent : IComponent;

		IComponent RemoveComponent(Type type);

		IComponent RemoveComponent(IComponent component);

		bool RemoveComponents();

		#endregion

		#endregion

		#region Sleeping

		/// <summary>
		/// Return whether this Entity.FreeSleeping > 0;
		/// </summary>
		bool IsFreeSleeping { get; set; }

		/// <summary>
		/// This Entity's free-sleeping count. If this Entity is free-sleeping,
		/// its sleeping is not changed by its parent sleeping.
		/// </summary>
		int FreeSleeping { get; }

		#endregion

		string AncestorsToString(int depth = -1, bool localNames = true, string indent = "");

		string DescendantsToString(int depth = -1, bool localNames = true, string indent = "");

		/// <summary>
		/// Returns a formatted and indented string of the <see cref="IEntity"/> hierarchy.
		/// </summary>
		/// <param name="depth">Adds children recursively to the output until the given depth. -1 is the entire hierarchy.</param>
		/// <param name="addComponents">Adds components to the output.</param>
		/// <param name="addManagers">Adds component entities to the output.</param>
		/// <param name="addSystems">Adds systems to the output.</param>
		/// <param name="indent"></param>
		/// <returns></returns>
		string ToInfoString(int depth = -1, bool addComponents = true, bool addManagers = false, string indent = "");
	}
}