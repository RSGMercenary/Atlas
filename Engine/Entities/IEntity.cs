using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Systems;
using Atlas.Interfaces;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Entities
{
	interface IEntity:IEngine<IEntity>, IHierarchy<IEntity>, ISleep, IDispose, IUnmanagedDispose
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

		int SleepingParentIgnored { get; set; }
		bool IsSleepingParentIgnored { get; }

		#region Components

		bool HasComponent<TInterface>() where TInterface : IComponent;
		bool HasComponent(Type type);

		TComponent GetComponent<TComponent, TType>() where TComponent : IComponent, TType;
		TType GetComponent<TType>() where TType : IComponent;
		IComponent GetComponent(Type type);

		TComponent AddComponent<TComponent, TImplementation>() where TComponent : IComponent, TImplementation, new();
		TComponent AddComponent<TComponent, TBase>(TComponent Component) where TComponent : IComponent, TBase;
		TComponent AddComponent<TComponent, TBase>(TComponent Component, int index) where TComponent : IComponent, TBase;
		TComponent AddComponent<TComponent>() where TComponent : IComponent, new();
		TComponent AddComponent<TComponent>(TComponent Component) where TComponent : IComponent;
		TComponent AddComponent<TComponent>(TComponent Component, int index) where TComponent : IComponent;
		IComponent AddComponent(IComponent component, Type type);
		IComponent AddComponent(IComponent component, int index);
		IComponent AddComponent(IComponent component, Type type = null, int index = int.MaxValue);

		IComponent RemoveComponent(IComponent component);
		TComponent RemoveComponent<TComponent, TInterface>() where TComponent : IComponent, TInterface;
		TInterface RemoveComponent<TInterface>() where TInterface : IComponent;
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

		IReadOnlyCollection<Type> Systems { get; }

		ISignal<IEntity, Type> SystemAdded { get; }
		ISignal<IEntity, Type> SystemRemoved { get; }

		#endregion

		string Dump(string indent = "");
	}
}
