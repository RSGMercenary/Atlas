using Atlas.Engine.Components;
using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Entities
{
	interface IEntity:IEngineObject<IEntity>, IHierarchy<IEntity, IEntity>, ISleepHierarchy<IEntity>, IAutoDispose, IReset
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

		bool HasComponent<TAbstraction>() where TAbstraction : IComponent;
		bool HasComponent(Type type);

		TComponent GetComponent<TComponent, TAbstraction>() where TComponent : IComponent, TAbstraction;
		TAbstraction GetComponent<TAbstraction>() where TAbstraction : IComponent;
		IComponent GetComponent(Type type);

		TComponent AddComponent<TComponent, TAbstraction>() where TComponent : IComponent, TAbstraction, new();
		TComponent AddComponent<TComponent, TAbstraction>(TComponent Component) where TComponent : IComponent, TAbstraction;

		TComponent AddComponent<TComponent, TAbstraction>(TComponent Component, int index) where TComponent : IComponent, TAbstraction;
		TComponent AddComponent<TComponent>() where TComponent : IComponent, new();
		TComponent AddComponent<TComponent>(TComponent Component) where TComponent : IComponent;
		TComponent AddComponent<TComponent>(TComponent Component, int index) where TComponent : IComponent;
		IComponent AddComponent(IComponent component, Type type);
		IComponent AddComponent(IComponent component, int index);
		IComponent AddComponent(IComponent component, Type type = null, int index = int.MaxValue);

		IComponent RemoveComponent(IComponent component);
		TComponent RemoveComponent<TComponent, TAbstraction>() where TComponent : IComponent, TAbstraction;
		TAbstraction RemoveComponent<TAbstraction>() where TAbstraction : IComponent;
		IComponent RemoveComponent(Type type);
		bool RemoveComponents();

		Type GetComponentType(IComponent component);
		IReadOnlyDictionary<Type, IComponent> Components { get; }

		ISignal<IEntity, IComponent, Type, IEntity> ComponentAdded { get; }
		ISignal<IEntity, IComponent, Type, IEntity> ComponentRemoved { get; }

		#endregion

		#region Systems

		bool HasSystemType(Type type);
		bool HasSystemType<TSystem>() where TSystem : ISystem;

		bool AddSystemType(Type type);
		bool AddSystemType<TSystem>() where TSystem : ISystem;

		bool RemoveSystemType(Type type);
		bool RemoveSystemType<TSystem>() where TSystem : ISystem;
		bool RemoveSystemTypes();

		IReadOnlyCollection<Type> SystemTypes { get; }

		ISignal<IEntity, Type> SystemTypeAdded { get; }
		ISignal<IEntity, Type> SystemTypeRemoved { get; }

		#endregion

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

		string HierarchyToString();

		/// <summary>
		/// Returns a formatted and indented string of the <see cref="IEntity"/> hierarchy.
		/// </summary>
		/// <param name="depth">Adds children recursively to the output until the given depth. -1 is the entire hierarchy.</param>
		/// <param name="addComponents">Adds components to the output.</param>
		/// <param name="addEntities">Adds component entities to the output.</param>
		/// <param name="addSystems">Adds systems to the output.</param>
		/// <param name="indent"></param>
		/// <returns></returns>
		string ToString(int depth = -1, bool addComponents = true, bool addEntities = false, bool addSystems = true, string indent = "");
	}
}