using Atlas.Interfaces.Components;
using Atlas.Interfaces.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Interfaces.Entities
{
	interface IEntity:ISleep
	{
		string UniqueName
		{
			get;
			set;
		}
		string Name
		{
			get;
			set;
		}

		IEntity Parent
		{
			get; set;
		}

		ISignal<IEntity, IEntity, IEntity> ParentChanged
		{
			get;
		}

		List<IEntity> Children
		{
			get;
		}

		int NumChildren
		{
			get;
		}

		IEntity GetChild(int index);
		IEntity GetChild(string name);

		IEntity AddChild(IEntity entity);
		IEntity AddChild(IEntity entity, int index);
		ISignal<IEntity, IEntity> ChildAdded
		{
			get;
		}

		IEntity RemoveChild(IEntity entity);
		IEntity RemoveChild(int index);
		IEntity RemoveChild(string name);

		List<IComponent> Components
		{
			get;
		}

		List<Type> ComponentTypes
		{
			get;
		}

		T GetComponent<T>() where T : IComponent;
		IComponent GetComponent(Type type);
		Type GetComponentType(IComponent component);

		T AddComponent<T>() where T : IComponent;
		T AddComponent<T>(T component) where T : IComponent;
		IComponent AddComponent(IComponent component);
		IComponent AddComponent(IComponent component, Type type);
		IComponent AddComponent(IComponent component, Type type, int index);

		T RemoveComponent<T>() where T : IComponent;
		IComponent RemoveComponent(Type type);
		IComponent RemoveComponent(IComponent component);
	}
}
