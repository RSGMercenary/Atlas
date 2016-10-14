using Atlas.Components;
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
		IEntityManager EntityManager { get; set; }
		Signal<IEntity, IEntityManager, IEntityManager> EntityManagerChanged { get; }

		string GlobalName { get; set; }
		Signal<IEntity, string, string> GlobalNameChanged { get; }

		string LocalName { get; set; }
		Signal<IEntity, string, string> LocalNameChanged { get; }

		IEntity Parent { get; set; }
		Signal<IEntity, IEntity, IEntity> ParentChanged { get; }
		Signal<IEntity, int, int> ParentIndexChanged { get; }

		bool SetParent(IEntity parent = null, int index = int.MaxValue);

		bool HasChild(IEntity entity);
		bool HasChild(string localName);

		int GetChildIndex(IEntity entity);
		bool SetChildIndex(IEntity entity, int index);
		Signal<IEntity, int, int, bool> ChildIndicesChanged { get; }

		IEntity Root { get; }
		IEntity GetHierarchy(string hierarchy);
		bool SetHierarchy(string hierarchy, int index);
		bool HasHierarchy(IEntity entity);

		IEntity GetChild(string localName);
		IEntity GetChild(int index);
		IReadOnlyLinkList<IEntity> Children { get; }

		IEntity AddChild(IEntity child);
		IEntity AddChild(IEntity child, int index);
		Signal<IEntity, IEntity, int> ChildAdded { get; }

		IEntity RemoveChild(IEntity child);
		IEntity RemoveChild(int index);
		void RemoveChildren();
		Signal<IEntity, IEntity, int> ChildRemoved { get; }

		int SleepingParentIgnored { get; set; }
		bool IsSleepingParentIgnored { get; }

		#region Components

		bool HasComponent(Type type);
		bool HasComponent<T>() where T : IComponent;

		IComponent GetComponent(Type type);
		T GetComponent<T>() where T : IComponent;

		T AddComponent<T>() where T : IComponent, new();
		T AddComponent<T>(T component) where T : IComponent;
		T AddComponent<T>(T component, Type type) where T : IComponent;
		T AddComponent<T>(T component, int index) where T : IComponent;
		T AddComponent<T>(T component, Type type, int index) where T : IComponent;

		IComponent AddComponent(IComponent component);
		IComponent AddComponent(IComponent component, Type type);
		IComponent AddComponent(IComponent component, int index);
		IComponent AddComponent(IComponent component, Type type, int index);
		Signal<IEntity, IComponent, Type> ComponentAdded { get; }

		IComponent RemoveComponent(IComponent component);
		IComponent RemoveComponent(Type type);
		T RemoveComponent<T>() where T : IComponent;
		Signal<IEntity, IComponent, Type> ComponentRemoved { get; }

		Type GetComponentType(IComponent component);
		IReadOnlyDictionary<Type, IComponent> Components { get; }

		#endregion

		#region Systems

		bool HasSystemType(Type type);
		bool HasSystemType<T>() where T : ISystem;

		bool AddSystemType(Type type);
		bool AddSystemType<T>() where T : ISystem;

		bool RemoveSystemType(Type type);
		bool RemoveSystemType<T>() where T : ISystem;

		Signal<IEntity, Type> SystemTypeAdded { get; }
		Signal<IEntity, Type> SystemTypeRemoved { get; }

		#endregion

		string Dump(string indent = "");
	}
}
