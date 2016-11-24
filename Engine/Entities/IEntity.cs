using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using Atlas.Interfaces;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Entities
{
	interface IEntity:IEngine<IEntity>, IHierarchy<IEntity>, ISleep, IDispose, IAutoDispose
	{
		#region Entities

		/// <summary>
		/// This Entity's global name. This name is unique to its Engine.
		/// If this Entity is added to an Engine, and this global name already
		/// exists, then this Entity's global name will be changed.
		/// </summary>
		string GlobalName { get; set; }

		/// <summary>
		/// This Entity's local name. This name is unique to its Parent.
		/// If this Entity is added to a parent, and this local name already
		/// exists, then this Entity's local name will be changed.
		/// </summary>
		string LocalName { get; set; }

		ISignal<IEntity, string, string> GlobalNameChanged { get; }

		ISignal<IEntity, string, string> LocalNameChanged { get; }

		bool HasChild(string localName);

		IEntity GetHierarchy(string hierarchy);

		bool SetHierarchy(string hierarchy, int index);

		IEntity GetChild(string localName);

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

		ISignal<IEntity, IComponent, Type> ComponentAdded { get; }
		ISignal<IEntity, IComponent, Type> ComponentRemoved { get; }

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

		/// <summary>
		/// This Entity's free-sleeping count. If this Entity is free-sleeping,
		/// its sleeping is not changed by its parent sleeping.
		/// </summary>
		int FreeSleeping { get; set; }

		ISignal<IEntity, int, int> FreeSleepingChanged { get; }

		/// <summary>
		/// Return whether this Entity.FreeSleeping > 0;
		/// </summary>
		bool IsFreeSleeping { get; }

		string ToString(bool includeChildren = true, bool includeComponents = true, bool includeSystems = true, string indent = "");
	}
}
