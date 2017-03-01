﻿using Atlas.Engine.Components;
using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Entities
{
	interface IEntity : IEngineObject<IEntity>, IHierarchy<IEntity>, ISleepHierarchy<IEntity>, IReset
	{
		#region Entities

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

		ISignal<IEntity, string, string> GlobalNameChanged { get; }

		ISignal<IEntity, string, string> LocalNameChanged { get; }

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

		bool SetHierarchy(string hierarchy, int index);

		IEntity GetChild(string localName);

		IEntity RemoveChild(string localName);

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
		TComponent AddComponent<TIComponent, TComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent, new();

		/// <summary>
		/// Adds a Component to the Entity with the given instance.
		/// </summary>
		/// <typeparam name="TIComponent">The interface Type of the Component.</typeparam>
		/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <returns></returns>
		TComponent AddComponent<TIComponent, TComponent>(TComponent component)
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
		TComponent AddComponent<TIComponent, TComponent>(TComponent component, int index)
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
		/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <returns></returns>
		TComponent AddComponent<TComponent>(TComponent component)
			where TComponent : IComponent;

		/// <summary>
		/// Adds a Component to the Entity with the given instance and index.
		/// </summary>
		/// <typeparam name="TComponent">The instance Type of the Component.</typeparam>
		/// <param name="component">The instance of the Component.</param>
		/// <param name="index">The index of the Entity within the Component.</param>
		/// <returns></returns>
		TComponent AddComponent<TComponent>(TComponent component, int index)
			where TComponent : IComponent;

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

		ISignal<IEntity, IComponent, Type, IEntity> ComponentAdded { get; }
		ISignal<IEntity, IComponent, Type, IEntity> ComponentRemoved { get; }

		#endregion

		#region Systems

		bool HasSystem(Type type);
		bool HasSystem<TSystem>() where TSystem : ISystem;

		bool AddSystem(Type type);
		bool AddSystem<TSystem>() where TSystem : ISystem;

		bool RemoveSystem(Type type);
		bool RemoveSystem<TSystem>() where TSystem : ISystem;
		bool RemoveSystems();

		IReadOnlyCollection<Type> Systems { get; }

		ISignal<IEntity, Type> SystemAdded { get; }
		ISignal<IEntity, Type> SystemRemoved { get; }

		#endregion

		#region Sleeping

		/// <summary>
		/// Return whether this Entity.FreeSleeping > 0;
		/// </summary>
		bool IsFreeSleeping { get; }

		/// <summary>
		/// This Entity's free-sleeping count. If this Entity is free-sleeping,
		/// its sleeping is not changed by its parent sleeping.
		/// </summary>
		int FreeSleeping { get; set; }

		ISignal<IEntity, int, int> FreeSleepingChanged { get; }

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
		string ToString(int depth = -1, bool addComponents = true, bool addManagers = false, bool addSystems = true, string indent = "");
	}
}