using Atlas.Components;
using Atlas.Engine;
using Atlas.Interfaces;
using Atlas.LinkList;
using Atlas.Signals;
using Atlas.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.Entities
{
	interface IEntity:ISleep, IDispose
	{
		IEngine Engine { get; set; }
		ISignal<IEntity, IEngine, IEngine> EngineChanged { get; }

		string GlobalName { get; set; }
		ISignal<IEntity, string, string> GlobalNameChanged { get; }

		string LocalName { get; set; }
		ISignal<IEntity, string, string> LocalNameChanged { get; }

		IEntity Parent { get; set; }
		ISignal<IEntity, IEntity, IEntity> ParentChanged { get; }
		ISignal<IEntity, int, int> ParentIndexChanged { get; }

		bool SetParent(IEntity parent = null, int index = int.MaxValue);

		bool HasChild(IEntity entity);
		bool HasChild(string localName);

		int GetChildIndex(IEntity entity);
		bool SetChildIndex(IEntity entity, int index);
		ISignal<IEntity, int, int, bool> ChildIndicesChanged { get; }

		IEntity Root { get; }
		IEntity GetHierarchy(string hierarchy);
		bool SetHierarchy(string hierarchy, int index);
		bool HasHierarchy(IEntity entity);

		IEntity GetChild(string localName);
		IEntity GetChild(int index);
		IReadOnlyLinkList<IEntity> Children { get; }

		IEntity AddChild(IEntity child);
		IEntity AddChild(IEntity child, int index);
		ISignal<IEntity, IEntity, int> ChildAdded { get; }

		IEntity RemoveChild(IEntity child);
		IEntity RemoveChild(int index);
		void RemoveChildren();
		ISignal<IEntity, IEntity, int> ChildRemoved { get; }

		int SleepingParentIgnored { get; set; }
		bool IsSleepingParentIgnored { get; }

		#region Components

		bool HasComponent<TType>() where TType : IComponent;
		bool HasComponent(Type type);

		TComponent GetComponent<TComponent, TType>() where TComponent : IComponent, TType;
		TType GetComponent<TType>() where TType : IComponent;
		IComponent GetComponent(Type type);

		TComponent AddComponent<TComponent, TType>() where TComponent : IComponent, TType, new();
		TComponent AddComponent<TComponent, TType>(TComponent Component) where TComponent : IComponent, TType;
		TComponent AddComponent<TComponent, TType>(TComponent Component, int index) where TComponent : IComponent, TType;
		TComponent AddComponent<TComponent>() where TComponent : IComponent, new();
		TComponent AddComponent<TComponent>(TComponent Component) where TComponent : IComponent;
		TComponent AddComponent<TComponent>(TComponent Component, int index) where TComponent : IComponent;
		IComponent AddComponent(IComponent component, Type type, int index);
		ISignal<IEntity, IComponent, Type> ComponentAdded { get; }

		IComponent RemoveComponent(IComponent component);
		TComponent RemoveComponent<TComponent, TType>() where TComponent : IComponent, TType;
		TType RemoveComponent<TType>() where TType : IComponent;
		IComponent RemoveComponent(Type type);
		bool RemoveComponents();
		ISignal<IEntity, IComponent, Type> ComponentRemoved { get; }

		Type GetComponentType(IComponent component);
		IReadOnlyDictionary<Type, IComponent> Components { get; }

		#endregion

		#region Systems

		bool HasSystem(Type type);
		bool HasSystem<T>() where T : ISystem;

		bool AddSystem(Type type);
		bool AddSystem<T>() where T : ISystem;

		bool RemoveSystem(Type type);
		bool RemoveSystem<T>() where T : ISystem;

		IReadOnlyCollection<Type> Systems { get; }

		ISignal<IEntity, Type> SystemAdded { get; }
		ISignal<IEntity, Type> SystemRemoved { get; }

		#endregion

		string Dump(string indent = "");
	}
}
